using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    using WinPoint = System.Windows.Point;
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var model = new SGLPlayerModel(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "X8PC00A.BIN"));
            var body = Draw(model.Meshes.BodyXPData, model);

            group.Children.Add(body);

            var weapon = Draw(model.Meshes.WeaponXPData, model);

            group.Children.Add(weapon);
        }

        public Model3DGroup Draw(List<XPData> xpdata, SGLPlayerModel model)
        {
            var whole = new Model3DGroup();

            //Order for Synbios Idle
            var transformTest = new List<int> { 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9,  0x15,  0x10, 0x11, 0x12, 0x13,  0x14,  0xA, 0xB, 0xC, 0xD, 0xE,  0xF };

            foreach (var b in xpdata)
            {
                var verticies = b.Verticies.Select(v => v.ToPoint3D()).ToArray();
                var part = new Model3DGroup();

                foreach (var f in b.Faces)
                {

                    var mesh = new MeshGeometry3D();
                    mesh.Positions.Add(verticies[f.Vertex0]);
                    mesh.Positions.Add(verticies[f.Vertex1]);
                    mesh.Positions.Add(verticies[f.Vertex2]);
                    mesh.Positions.Add(verticies[f.Vertex3]);
                    mesh.TriangleIndices.Add(0);
                    mesh.TriangleIndices.Add(1);
                    mesh.TriangleIndices.Add(2);
                    mesh.TriangleIndices.Add(2);
                    mesh.TriangleIndices.Add(3);
                    mesh.TriangleIndices.Add(0);
                    var attr = f.Attribute;
                    if (attr.ZOrder > 0) ;//TODO: Do something with ZOrder

                    Brush brush = new SolidColorBrush(Global.ToColor(attr.ColorNumber));

                    if (attr.IsTexture)
                    {

                        if (attr.IsVerticalFlip && attr.IsHorizontalFlip)
                        {
                            
                            mesh.TextureCoordinates.Add(new WinPoint(1, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 0));

                        }
                        else if (attr.IsVerticalFlip)
                        {
                            mesh.TextureCoordinates.Add(new WinPoint(0, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 0));

                        }
                        else if (attr.IsHorizontalFlip)
                        {
                            mesh.TextureCoordinates.Add(new WinPoint(1, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 1));

                        }
                        else
                        {
                            mesh.TextureCoordinates.Add(new WinPoint(0, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 0));
                            mesh.TextureCoordinates.Add(new WinPoint(1, 1));
                            mesh.TextureCoordinates.Add(new WinPoint(0, 1));

                        }
                        var def = model.TextureDefinitions.Definitions[attr.TextureNum];
                        brush = new ImageBrush() { ImageSource = BitmapToImageSource(def.ToBitmap()) };
                    }
                    mesh.Normals.Add(f.Normal.ToVector3D());
                    var mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(brush));
                    mGeometry.BackMaterial = new DiffuseMaterial(brush);

                    part.Children.Add(mGeometry);
                }
                
                var events = model.Animations.AnimationEvents;


              //  var matrix = model.Animations.6(transformTest[xpdata.IndexOf(b) ], 1);
                //transform.Children.Add(Transform3D.Identity);

               // var t = matrix.ToTranslateTransform3D();
                //var transform = new Transform3DGroup();
                //transform.Children.Add(matrix.ToScaleTransform3D());
                //transform.Children.Add(matrix.ToRotateTransform3D());
                //transform.Children.Add(matrix.ToTranslateTransform3D());

                //part.Transform = transform;

                whole.Children.Add(part);
            }
            return whole;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        ImageSource BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            var h = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(h, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(h);
            }
        }
    }
}

