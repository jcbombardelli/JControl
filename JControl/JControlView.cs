using NGraphics;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace JControl.Abstraction
{
    public class JControlView : ContentView
    {
        #region Events

        /// <summary>
        /// Occurs when on invalidate.
        /// </summary>
        public event EventHandler OnInvalidate;

        /// <summary>
        /// Touches began
        /// </summary>
        public event EventHandler<IEnumerable<NGraphics.Point>> OnTouchesBegan;

        /// <summary>
        /// Touches moved
        /// </summary>
        public event EventHandler<IEnumerable<NGraphics.Point>> OnTouchesMoved;

        /// <summary>
        /// Touches ended
        /// </summary>
        public event EventHandler<IEnumerable<NGraphics.Point>> OnTouchesEnded;

        /// <summary>
        /// Touches cancelled
        /// </summary>
        public event EventHandler<IEnumerable<NGraphics.Point>> OnTouchesCancelled;

        #endregion

        #region Delegates and Callbacks

        /// <summary>
        /// Get platform delegate
        /// </summary>
        public delegate IPlatform GetPlatformDelegate();

        /// <summary>
        /// Occurs when on create canvas.
        /// </summary>
        public event GetPlatformDelegate OnGetPlatform;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NControl.Forms.Xamarin.Plugins.iOS.NControlNativeView"/> class.
        /// </summary>
        public JControlView()
        {
            BackgroundColor = Xamarin.Forms.Color.Transparent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NControl.Forms.Xamarin.Plugins.iOS.NControlNativeView"/> class with
        /// a callback action for drawing
        /// </summary>
        /// <param name="drawingFunc">Drawing func.</param>
        public JControlView(Action<ICanvas, Rect> drawingFunc): this()
		{
            DrawingFunction = drawingFunc;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the drawing function.
        /// </summary>
        /// <value>The drawing function.</value>	    
        public Action<ICanvas, Rect> DrawingFunction { get; set; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public IPlatform Platform
        {
            get
            {
                if (OnGetPlatform == null)
                    throw new ArgumentNullException("OnGetPlatform");

                return OnGetPlatform();
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Invalidate this instance.
        /// </summary>
        public void Invalidate()
        {
            if (OnInvalidate != null)
                OnInvalidate(this, EventArgs.Empty);
        }

        /// <summary>
        /// Draw the specified canvas.
        /// </summary>
        /// <param name="canvas">Canvas.</param>
        public virtual void Draw(ICanvas canvas, Rect rect)
        {
            if (DrawingFunction != null)
                DrawingFunction(canvas, rect);
        }

        #endregion

        #region Touches

        /// <summary>
        /// Touchs down.
        /// </summary>
        /// <param name="point">Point.</param>
        public virtual bool TouchesBegan(IEnumerable<NGraphics.Point> points)
        {
            if (OnTouchesBegan != null)
            {
                OnTouchesBegan(this, points);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toucheses the moved.
        /// </summary>
        /// <param name="point">Point.</param>
        public virtual bool TouchesMoved(IEnumerable<NGraphics.Point> points)
        {
            if (OnTouchesMoved != null)
            {
                OnTouchesMoved(this, points);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toucheses the cancelled.
        /// </summary>
        public virtual bool TouchesCancelled(IEnumerable<NGraphics.Point> points)
        {
            if (OnTouchesCancelled != null)
            {
                OnTouchesCancelled(this, points);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toucheses the ended.
        /// </summary>
        public virtual bool TouchesEnded(IEnumerable<NGraphics.Point> points)
        {
            if (OnTouchesEnded != null)
            {
                OnTouchesEnded(this, points);
                return true;
            }

            return false;
        }

        #endregion
    }
}
