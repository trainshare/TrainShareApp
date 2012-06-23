using System.Windows;

namespace TrainShareApp.Extension
{
    public static class LinqToXaml
    {
        public static FrameworkElement Parent<T>(this FrameworkElement source)
             where T : FrameworkElement
         {
             var node = source;

             while (node != null)
             {
                 if (node is T) return node as T;

                 node = node.Parent as FrameworkElement;
             }

             return null;
         }
    }
}