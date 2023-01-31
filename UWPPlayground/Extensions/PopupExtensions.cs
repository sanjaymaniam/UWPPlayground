using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace UWPPlayground.Extensions
{
	public enum Side { Left, Top, Right, Bottom }

	public static class PreferenceOrders
	{
		public static Side[] TopBottomLeft = new Side[] { Side.Top, Side.Bottom, Side.Left };
		public static Side[] TopBottomLeftRight = new Side[] { Side.Top, Side.Bottom, Side.Left, Side.Right };
		public static Side[] Left = new Side[] { Side.Left };
	}

	public static class PopupExtension
	{
		/// <summary>
		/// Use this method to show a popup near another FrameworkElement.
		/// <para>NOTE: MaxWidth and MaxHeight of the popup must be set beforehand.</para>
		/// </summary>
		/// <param name="targetElement">
		/// Framework element to position popup near.
		/// </param>
		/// <param name="preferenceOrder">
		/// Represents the preferred order of <see cref="Side"/> to position popup. 
		/// For eg: If preferenceOrder is { Side.Left, Side.Right }, extension will first try to position the popup to left 
		/// of <paramref name="targetElement"/> and if that's not possible, it tries to position popup to the right.
		/// </param>
		/// <returns>
		/// True when popup has been positioned in any of the sides in <paramref name="preferenceOrder"/>. False otherwise.
		/// </returns>
		public static bool TryShowNear(this Popup popup, FrameworkElement targetElement, Side[] preferenceOrder, double margin = 10, bool isOverflowAllowed = false)
		{
			try
			{
				// TO DO: Handle case where popup can be shown outside WindowBounds.

				// Tranform (0, 0) of popup and target element with respect to root page.
				var popupTransform = popup.TransformToVisual(Window.Current.Content);
				Point popupCoords = popupTransform.TransformPoint(new Point(0, 0));
				var targetElementTransform = targetElement.TransformToVisual(Window.Current.Content);
				Point targetElementCoords = targetElementTransform.TransformPoint(new Point(0, 0));

				// Calculate horizontal and vertical distance between popup and target element.
				double distanceX = targetElementCoords.X - popupCoords.X; // distanceX > 0 : popup is to left of target element
				double distanceY = targetElementCoords.Y - popupCoords.Y; // distanceY > 0 : popup is above target element

				Rect windowBounds = Window.Current.Bounds;
				double? verticalOffsetForAlignment = null;
				double? horizontalOffsetForAlignment = null;

				foreach (var side in preferenceOrder)
				{
					switch (side)
					{
						case Side.Left:
							// Calculates the horizontal offset needed to position the popup completely to the left side of the target element.
							var horizontalOffset = distanceX - popup.MaxWidth - margin;
							// If the calculated offset positions popup within window boundaries, set the horizontal offset and align popup vertically.
							if (popupCoords.X + horizontalOffset >= 0)
							{
								popup.HorizontalOffset = horizontalOffset;
								if (!verticalOffsetForAlignment.HasValue)
								{
									verticalOffsetForAlignment = GetOffsetForAlignment(popupCoords.Y, 
																					   targetElementCoords.Y, 
																					   distanceY, 
																					   popup.MaxHeight, 
																					   targetElement.ActualHeight, 
																					   windowBounds.Height, 
																					   isOverflowAllowed);
									if (verticalOffsetForAlignment == null)
									{
										return false;
									}
								}
								popup.VerticalOffset = verticalOffsetForAlignment.Value;
								popup.IsOpen = true;
								return true;
							}
							continue;

						case Side.Right:
							horizontalOffset = distanceX + targetElement.ActualWidth + margin;
							if (popupCoords.X + horizontalOffset + popup.MaxWidth <= windowBounds.Width)
							{
								popup.HorizontalOffset = horizontalOffset;
								if (!verticalOffsetForAlignment.HasValue)
								{
									verticalOffsetForAlignment = GetOffsetForAlignment(popupCoords.Y, 
																					   targetElementCoords.Y, 
																					   distanceY, 
																					   popup.MaxHeight, 
																					   targetElement.ActualHeight, 
																					   windowBounds.Height, 
																					   isOverflowAllowed);
									if (verticalOffsetForAlignment == null)
									{
										return false;
									}
								}
								popup.VerticalOffset = verticalOffsetForAlignment.Value;
								popup.IsOpen = true;
								return true;
							}
							continue;

						case Side.Top:
							// Calculates the vertical offset needed to position the popup completely above of the target element.
							var verticalOffset = distanceY - popup.MaxHeight - margin;
							// If the calculated offset positions popup within window boundaries, set the vertical offset and align popup horizontally.
							if (popupCoords.Y + verticalOffset >= 0)
							{
								popup.VerticalOffset = verticalOffset;
								if (!horizontalOffsetForAlignment.HasValue)
								{
									horizontalOffsetForAlignment = GetOffsetForAlignment(popupCoords.X,
																						 targetElementCoords.X,
																						 distanceX,
																						 popup.MaxWidth,
																						 targetElement.ActualWidth,
																						 windowBounds.Width,
																						 isOverflowAllowed);
									if (horizontalOffsetForAlignment == null)
									{
										return false;
									}
								}
								popup.HorizontalOffset = horizontalOffsetForAlignment.Value;
								popup.IsOpen = true;
								return true;
							}
							continue;

						case Side.Bottom:
							verticalOffset = distanceY + targetElement.ActualHeight + margin;
							if (popupCoords.Y + verticalOffset + popup.MaxHeight <= windowBounds.Height)
							{
								popup.VerticalOffset = verticalOffset;
								if (!horizontalOffsetForAlignment.HasValue)
								{
									horizontalOffsetForAlignment = GetOffsetForAlignment(popupCoords.X, 
																						 targetElementCoords.X, 
																						 distanceX, 
																						 popup.MaxWidth, 
																						 targetElement.ActualWidth, 
																						 windowBounds.Width, 
																						 isOverflowAllowed);
									if (horizontalOffsetForAlignment == null)
									{
										return false;
									}
								}
								popup.HorizontalOffset = horizontalOffsetForAlignment.Value;
								popup.IsOpen = true;
								return true;
							}
							continue;
					}
				}
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		/// <summary>
		/// This method calculates the offset needed to align a popup with a target element.
		/// </summary>
		/// <param name="popupCoord"></param>
		/// <param name="targetElementCoord"></param>
		/// <param name="distance"></param>
		/// <param name="popupMaxDimension"></param>
		/// <param name="targetElementDimension"></param>
		/// <param name="windowDimension"></param>
		/// <returns></returns>
		private static double? GetOffsetForAlignment(double popupCoord, 
													 double targetElementCoord, 
													 double distance, 
													 double popupMaxDimension, 
													 double targetElementDimension, 
													 double windowDimension, 
													 bool isOverflowAllowed = false)
		{
			// Calculates the offset needed to align the center of the popup with the center of the target element.
			double offsetForCenterAlignment = distance - (popupMaxDimension - targetElementDimension) / 2;

			// If the calculated offset positions the popup within the window boundaries, align the center of the popup with the center of the target element.
			if (isOverflowAllowed ||
			   (popupCoord + offsetForCenterAlignment >= 0 && popupCoord + offsetForCenterAlignment + popupMaxDimension <= windowDimension))
			{
				return offsetForCenterAlignment;
			}
			else
			{

				// Check if aligning left/top causes overflow. Assumes left/top of targetElement is within window bounds.
				if (popupCoord + distance + popupMaxDimension <= windowDimension)
				{
					return distance;
				}
				// Check if aligning right/bottom causes overflow. Assumes right/bottom of targetElement is within window bounds.
				else if (popupCoord + distance - popupMaxDimension >= 0)
				{
					return distance - popupMaxDimension + targetElementDimension;
				}
			}
			return null;
		}
	}
}
