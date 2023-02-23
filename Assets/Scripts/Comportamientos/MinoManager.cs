/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

namespace UCM.IAV.Navegacion
{

    public class MinoManager : MonoBehaviour
    {
        public GameObject minotaur;

        private Graph graph;

        public int numMinos = 1;

        private void Start()
        {
            numMinos = GameManager.instance.getNumMinos();
            StartUp();
        }

        void StartUp()
        {
            GameObject graphGO = GameObject.Find("GraphGrid");

            if (graphGO != null)
                graph = graphGO.GetComponent<GraphGrid>();

            for (int i = 0; i < numMinos; i++)
                GenerateMino();
        }

        void GenerateMino()
        {

            GameObject minoGO = Instantiate(minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
    }
}
