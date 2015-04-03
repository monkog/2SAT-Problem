namespace SAT2
{
    public class Edge
    {
        #region Private Member
        private Vertex _from;
        private Vertex _to;
        #endregion Private Member
        #region Public Properties
        /// <summary>
        /// Gets the start vertex of the egde.
        /// </summary>
        public Vertex From { get { return _from; } }
        /// <summary>
        /// Gets the end vertex of the egde.
        /// </summary>
        public Vertex To { get { return _to; } }
        #endregion Public Properties
        #region Constructor
        public Edge(Vertex from, Vertex to)
        {
            _from = from;
            _to = to;
        }
        #endregion Constructor
    }
}

