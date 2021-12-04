using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    class PopulationController : MonoBehaviour
    {
        [SerializeField] private Car car_to_create;
        [SerializeField] private int size = 20;
        [SerializeField] private float mutate_chance = 0.01f;
        private Car[] population;
        private bool isNeedRetart;

        void Start()
        {
            car_to_create.transform.localPosition = transform.localPosition;
            population = new Car[size];

            for (int i = 0; i < population.Length; i++)
            {
                population[i] = Instantiate(car_to_create, transform.parent);
                population[i].CreateMLP();
                population[i].RandomW(1);
            }
        }

        void Update()
        {
            isNeedRetart = true;
            foreach (Car car in population)
            {
                if (!car.isNeedRestart())
                    isNeedRetart = false;
            }

            if (!isNeedRetart)
                return;

            MLP[,] best_fam = GetBestMLP();

            foreach (Car car in population)
            {
                Destroy(car.gameObject);
            }

            CreatePopulation(best_fam);
            isNeedRetart = false;
        }

        private void CreatePopulation(MLP[,] parent_fam)
        {
            for (int i = 0; i < population.Length; i++)
            {
                population[i] = Instantiate(car_to_create, transform.parent);
                population[i].SetW(parent_fam);
                population[i].RandomW(mutate_chance);
            }
            population[0].SetW(parent_fam);
            population[0].GetComponent<Image>().color = Color.red;
            population[0].transform.SetAsLastSibling();
        }

        private MLP[,] GetBestMLP()
        {
            double best_score = population[0].getScore();
            MLP[,] best_fam = population[0].perceptron;
            foreach (Car car in population)
            {
                if (car.getScore() > best_score)
                {
                    best_score = car.getScore();
                    best_fam = car.perceptron;
                }
            }
            Debug.Log(population[0].getScore() + " , " + best_score);
            return best_fam;
        }
    }
}
