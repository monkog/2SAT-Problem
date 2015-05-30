using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using SAT2.Properties;

namespace SAT2
{
    public class Sat2Problem
    {
        #region Private Members
        private readonly Dictionary<string, Vertex> _vertices;
        private DispatcherTimer _timer;
        private int _ticks;
        #endregion Private Members
        #region Public Properties
        public Dictionary<string, Vertex> Vertices { get { return _vertices; } }
        #endregion Public Properties
        #region Constructors
        public Sat2Problem()
        {
            _vertices = new Dictionary<string, Vertex>();
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 1) };
            _ticks = 0;
            _timer.Tick += (sender, args) => { _ticks++; };
        }
        #endregion Constructors
        #region Private Methods
        private static XmlNodeList GetFormulasFromFile(string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            if (xml.DocumentElement == null)
                throw new Exception("DocumentElement is null");

            var formulas = xml.DocumentElement.SelectNodes(Resources.Sat2ConditionNodeName);
            return formulas;
        }
        private void AddEdgeFromFormula(XmlNode x, XmlNode y)
        {
            Vertex from, to;
            bool isNegative = x.InnerText.StartsWith("-");
            string xName = isNegative ? x.InnerText.Substring(1) : "-" + x.InnerText;
            if (!Vertices.TryGetValue(xName, out from))
            {
                from = new Vertex(xName);
                Vertices.Add(xName, from);
                Vertex negation;

                if (isNegative)
                    Vertices.Add("-" + xName, negation = new Vertex("-" + xName));
                else
                    Vertices.Add(xName.Substring(1), negation = new Vertex(xName.Substring(1)));
                from.Negation = negation;
                negation.Negation = from;
            }

            isNegative = y.InnerText.StartsWith("-");
            string yName = y.InnerText;
            if (!Vertices.TryGetValue(yName, out to))
            {
                to = new Vertex(yName);
                Vertices.Add(yName, to);
                Vertex negation;

                if (isNegative)
                    Vertices.Add(yName.Substring(1), negation = new Vertex(yName.Substring(1)));
                else
                    Vertices.Add("-" + yName, negation = new Vertex("-" + yName));
                to.Negation = negation;
                negation.Negation = to;
            }

            from.Neighbours.Add(to);
            to.Negation.Neighbours.Add(from.Negation);
        }
        private void CreateGraph(XmlNodeList formulas)
        {
            foreach (XmlNode formula in formulas)
            {
                if (formula.Attributes == null)
                    throw new Exception("No attributes");

                var x = formula.Attributes.GetNamedItem("x");
                var y = formula.Attributes.GetNamedItem("y");

                AddEdgeFromFormula(x, y);
            }
        }
        private bool FindValuations()
        {
            var vertex = FindFirstVertex();
            if (vertex == null) return false;
            if (!ValuateGraphFromVertex(vertex.Value.Value)) return false;

            foreach (var v in Vertices.Where(x => !x.Value.IsSet))
                if (!ValuateGraphFromVertex(v.Value)) return false;

            return true;
        }
        private bool ValuateGraphFromVertex(Vertex vertex)
        {
            vertex.Value = true;

            foreach (var neighbour in vertex.Neighbours)
            {
                if ((neighbour.IsSet && !neighbour.Value) || (neighbour.Negation.IsSet && neighbour.Negation.Value))
                    return false;

                if (!neighbour.IsSet)
                {
                    neighbour.Value = true;
                    ValuateGraphFromVertex(neighbour);
                }
                if (!neighbour.Negation.IsSet)
                    neighbour.Negation.Value = false;
            }
            return true;
        }
        /// <summary>
        /// Finds the first vertex that does not have a path to its negated value
        /// </summary>
        /// <returns>Found vertex or null</returns>
        private KeyValuePair<string, Vertex>? FindFirstVertex()
        {
            foreach (var v in Vertices.Where(x => !x.Value.IsSet))
                if (!CheckExistingPath(v.Value, v.Value.Negation))
                    return v;

            return null;
        }
        /// <summary>
        /// Checks whethe the path between two given vertices exists
        /// </summary>
        /// <param name="vertex">Start vertex</param>
        /// <param name="negation">Destination vertex</param>
        /// <returns>True if the path from the start to the destination exists, otherwise false</returns>
        private bool CheckExistingPath(Vertex vertex, Vertex negation)
        {
            if (vertex.Checked)
                return false;
            vertex.Checked = true;
            if (vertex == negation)
                return true;
            foreach (var neighbour in vertex.Neighbours)
                if (CheckExistingPath(neighbour, negation))
                    return true;

            return false;
        }
        private void CreateResultFile(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(Resources.Sat2RootNodeName));
            var root = xml.SelectSingleNode(Resources.Sat2RootNodeName);

            foreach (var vertex in Vertices.Values.Where(x => !x.Name.StartsWith("-")))
            {
                var solution = xml.CreateElement(Resources.SolutionNode);
                solution.SetAttribute(Resources.VarNode, vertex.Name);
                solution.SetAttribute(Resources.ValueNode, (vertex.Value ? 1 : 0).ToString());
                root.AppendChild(solution);
            }

            xml.Save(fileName + Resources.SolutionFileNameSuffix);
        }
        #endregion Private Methods
        #region Public Methods
        /// <summary>
        /// Calculates the 2SAT problem from the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void Run(string fileName)
        {
            var formulas = GetFormulasFromFile(fileName);
            _timer.Start();
            CreateGraph(formulas);
            bool result = FindValuations();
            _timer.Stop();
            if (result)
            {
                CreateResultFile(fileName);
                MessageBox.Show("Done. Time ellapsed: " + _ticks / 100 + ":" + _ticks % 100);
            }
            else
                MessageBox.Show("No answers. Time ellapsed: " + _ticks / 100 + ":" + _ticks % 100);
        }
        #endregion Public Methods
    }
}

