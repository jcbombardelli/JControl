using System;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using JControl.Abstraction;
using NGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(JControlView), typeof(JControl.Droid.JControlViewRenderer))]
namespace JControl.Droid
{


    /// <summary>
    /// NControlView renderer.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class JControlViewRenderer : VisualElementRenderer<JControlView>
    {
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        /// <summary>
        /// Raises the element changed event.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<JControlView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                e.OldElement.OnInvalidate -= HandleInvalidate;
                e.OldElement.OnGetPlatform -= OnGetPlatformHandler;
            }

            if (e.NewElement != null)
            {
                e.NewElement.OnInvalidate += HandleInvalidate;
                e.NewElement.OnGetPlatform += OnGetPlatformHandler;
            }

            // Lets have a clear background
            this.SetBackgroundColor(Android.Graphics.Color.Transparent);

            Invalidate();
        }

        /// <summary>
        /// Override to avoid setting the background to a given color
        /// </summary>
        protected override void UpdateBackgroundColor()
        {
            // Do NOT call update background here.
            // base.UpdateBackgroundColor();
        }

        /// <summary>
        /// Raises the element property changed event.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == JControlView.BackgroundColorProperty.PropertyName)
                Element.Invalidate();
            else if (e.PropertyName == JControlView.IsVisibleProperty.PropertyName)
                Element.Invalidate();
        }

        #region Native Drawing 

        /// <Docs>The Canvas to which the View is rendered.</Docs>
        /// <summary>
        /// Draw the specified canvas.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        public override void Draw(Android.Graphics.Canvas canvas)
        {
            if (Element == null)
            {
                base.Draw(canvas);
                return;
            }

            // Draws the background and default android setup. Children will also be redrawn here
            // base.Draw(canvas);

            // Set up clipping
            if (Element.IsClippedToBounds)
                canvas.ClipRect(new Android.Graphics.Rect(0, 0, Width, Height));

            // Perform custom drawing from the NGraphics subsystems
            var ncanvas = new CanvasCanvas(canvas);

            var rect = new NGraphics.Rect(0, 0, Width, Height);

            // Fill background 
            ncanvas.FillRectangle(rect, new NGraphics.Color(Element.BackgroundColor.R, Element.BackgroundColor.G, Element.BackgroundColor.B, Element.BackgroundColor.A));

            // Custom drawing
            Element.Draw(ncanvas, rect);

            // Redraw children - since we might have a composite control containing both children 
            // and custom drawing code, we want children to be drawn last. The reason for this double-
            // drawing is that the base.Draw(canvas) call will handle background which is needed before
            // doing NGraphics drawing - but unfortunately it also draws children - which then will 
            // be drawn below NGraphics drawings.
            base.Draw(canvas);
        }

        #endregion

        #region Touch Handling

        /// <Docs>The motion event.</Docs>
        /// <returns>To be added.</returns>
        /// <para tool="javadoc-to-mdoc">Implement this method to handle touch screen motion events.</para>
        /// <format type="text/html">[Android Documentation]</format>
        /// <since version="Added in API level 1"></since>
        /// <summary>
        /// Raises the touch event event.
        /// </summary>
        /// <param name="e">E.</param>
        public override bool OnTouchEvent(MotionEvent e)
        {
            var scale = Element.Width / Width;

            var touchInfo = new[]{
                new NGraphics.Point(e.GetX() * scale, e.GetY() * scale)
            };

            var result = false;

            // Handle touch actions
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    if (Element != null)
                        result = Element.TouchesBegan(touchInfo);
                    break;

                case MotionEventActions.Move:
                    if (Element != null)
                        result = Element.TouchesMoved(touchInfo);
                    break;

                case MotionEventActions.Up:
                    if (Element != null)
                        result = Element.TouchesEnded(touchInfo);
                    break;

                case MotionEventActions.Cancel:
                    if (Element != null)
                        result = Element.TouchesCancelled(touchInfo);
                    break;
            }

            return result;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Handles the invalidate.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Arguments.</param>
        private void HandleInvalidate(object sender, System.EventArgs args)
        {
            Invalidate();
        }

        /// <summary>
        /// Callback for the OnGetPlatform event in the abstract control
        /// </summary>
        private IPlatform OnGetPlatformHandler()
        {
            return new AndroidPlatform();
        }

        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <returns>The screen size.</returns>
        protected Xamarin.Forms.Size GetScreenSize()
        {
            var metrics = Forms.Context.Resources.DisplayMetrics;
            return new Xamarin.Forms.Size(metrics.WidthPixels, metrics.HeightPixels);
        }
        #endregion
    }
}
