/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/

using System;

namespace UCM.IAV.Navegacion
{
    public class Node : IComparable<Node>
    {

        public int vertexId; // current path vertex id
        public int prevVertId; // prev vertex id
        public float costSoFar; // cost from temp path made
        public float estimatedTotalCost; // estimate cost 

        public int CompareTo(Node other)
        {
            return (int)(this.estimatedTotalCost - other.estimatedTotalCost);
        }

        public bool Equals(Node other)
        {
            return (other.vertexId == this.vertexId);
        }

        public override bool Equals(object obj)
        {
            Node other = (Node)obj;
            return (other.vertexId == this.vertexId);
        }

        public override int GetHashCode()
        {
            return this.vertexId.GetHashCode();
        }

    }
}