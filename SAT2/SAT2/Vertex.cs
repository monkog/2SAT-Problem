namespace SAT2
{
    public class Vertex
    {
        #region Private Members
        private bool _isSet;
        private bool _value;
        private string _name;
        #endregion Private Members
        #region Public Properties
        /// <summary>
        /// Gets the value indicating whether the value of the variable was already calculated.
        /// </summary>
        public bool IsSet { get { return _isSet; } }
        /// <summary>
        /// Gets or sets a value of the variable.
        /// </summary>
        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isSet = true;
            }
        }
        public string Name { get { return _name; } }
        #endregion Public Properties
        #region Constructors
        public Vertex(string name)
        {
            _value = false;
            _isSet = false;
            _name = name;
        }
        #endregion Constructors
    }
}

