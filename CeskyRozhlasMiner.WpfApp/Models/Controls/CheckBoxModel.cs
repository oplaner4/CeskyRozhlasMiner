namespace CeskyRozhlasMiner.WpfApp.Models.Controls
{
    public class CheckBoxModel<T>
    {
        public bool IsChecked { get; set; }
        public string Label { get; set; }
        public T Value { get; set; }
    }
}
