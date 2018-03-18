namespace empilab
{
    public class Model
    {
        private readonly double _x;
        private readonly double _y;
        public Model(double x, double y)
        {
            _x = x;
            _y = y;
        }
        public double X => _x;
        public double Y => _y;
    }
}
