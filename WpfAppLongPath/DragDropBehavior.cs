using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WpfAppLongPath
{
  public static class DragDropBehavior
  {
    #region CanDrag property
    public static readonly DependencyProperty CanDragProperty = DependencyProperty.RegisterAttached(
        "CanDrag", typeof(bool), typeof(DragDropBehavior), new PropertyMetadata(false));

    public static bool GetCanDrag(DependencyObject dependencyObject)
    {
      return (bool)dependencyObject.GetValue(CanDragProperty);
    }

    public static void SetCanDrag(DependencyObject dependencyObject, bool value)
    {
      dependencyObject.SetValue(CanDragProperty, value);
    }

    private static void OnPreviewMouseDown(object sender, MouseEventArgs e)
    {
      if (sender is DependencyObject dependencyObject)
      {
        var isDragSource = GetIsDragSource(dependencyObject);
        SetCanDrag(dependencyObject, isDragSource);
      }
    }
    #endregion

    #region IsDragSource property
    public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached(
        "IsDragSource", typeof(bool), typeof(DragDropBehavior), new PropertyMetadata(false, OnIsDragSourcePropertyChanged));

    public static bool GetIsDragSource(DependencyObject dependencyObject)
    {
      return (bool)dependencyObject.GetValue(IsDragSourceProperty);
    }

    public static void SetIsDragSource(DependencyObject dependencyObject, bool value)
    {
      dependencyObject.SetValue(IsDragSourceProperty, value);
    }

    private static void OnMouseMove(object sender, MouseEventArgs e)
    {
      if (sender is FrameworkElement frameworkElement)
      {
        var canDrag = GetCanDrag(frameworkElement);
        if (canDrag && e.LeftButton == MouseButtonState.Pressed)
        {
          DataObject dragData = new DataObject(frameworkElement.DataContext.GetType(), frameworkElement.DataContext);
          DragDrop.DoDragDrop(frameworkElement, dragData, DragDropEffects.Copy);
          SetCanDrag(frameworkElement, false);
        }
      }
    }

    private static void OnIsDragSourcePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      if (dependencyObject is FrameworkElement frameworkElement)
      {
        var isDragSource = (bool)e.NewValue;
        if (isDragSource)
        {
          frameworkElement.PreviewMouseDown += OnPreviewMouseDown;
          frameworkElement.MouseMove += OnMouseMove;
        }
        else
        {
          frameworkElement.PreviewMouseDown -= OnPreviewMouseDown;
          frameworkElement.MouseMove -= OnMouseMove;
        }
      }
    }
    #endregion

    #region DropCommand property
    public static readonly DependencyProperty DropCommandProperty = DependencyProperty.RegisterAttached(
        "DropCommand", typeof(ICommand), typeof(DragDropBehavior), new PropertyMetadata(OnDropCommandPropertyChanged));

    public static ICommand GetDropCommand(DependencyObject dependencyObject)
    {
      return (ICommand)dependencyObject.GetValue(DropCommandProperty);
    }

    public static void SetDropCommand(DependencyObject dependencyObject, ICommand value)
    {
      dependencyObject.SetValue(DropCommandProperty, value);
    }

    private static void OnPreviewDragOver(object sender, DragEventArgs e)
    {
      var dropCommand = GetDropCommand(sender as DependencyObject);
      var canExecute = dropCommand?.CanExecute(e.Data) ?? false;
      e.Effects = canExecute ? DragDropEffects.Copy : DragDropEffects.None;
      e.Handled = canExecute;
    }

    private static void OnDrop(object sender, DragEventArgs e)
    {
      var dropCommand = GetDropCommand(sender as DependencyObject);
      if (dropCommand?.CanExecute(e.Data) ?? false)
      {
        dropCommand.Execute(e.Data);
        e.Handled = true;
      }
    }

    private static void OnDropCommandPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
      if (dependencyObject is FrameworkElement frameworkElement)
      {
        var dropCommand = e.NewValue;
        if (dropCommand != null)
        {
          frameworkElement.AllowDrop = true;
          frameworkElement.PreviewDragOver += OnPreviewDragOver;
          frameworkElement.Drop += OnDrop;
        }
        else
        {
          frameworkElement.AllowDrop = false;
          frameworkElement.PreviewDragOver -= OnPreviewDragOver;
          frameworkElement.Drop -= OnDrop;
        }
      }
    }
    #endregion
  }
}
