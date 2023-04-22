using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    public class ObstacleSpawner : MonoBehaviour
    {

        void Awake()
        {
            var planets = GameObject.FindGameObjectsWithTag("Planet");
            foreach (var planet in planets)
            {
                InstantiateObstacles(planet);
            }
        }

        void InstantiateObstacles(GameObject p)
        {
            p.TryGetComponent<Planet>(out var planet);

            Vector2 center = p.transform.position;
            for (int i = 0; i < planet.NumberOfObstacles; i++)
            {
                GameObject newGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float angle = i * Mathf.PI * 2f / planet.NumberOfObstacles;
                float x = Mathf.Cos(angle) * planet.PlanetSize;
                float y = Mathf.Sin(angle) * planet.PlanetSize;
                Vector2 point = new Vector2(x, y) + center;
                newGameObject.transform.position = new Vector3(x, y, 1);
                newGameObject.transform.parent = p.transform;
            }

        }

    }
}

