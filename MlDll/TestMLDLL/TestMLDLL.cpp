#include <iostream>
#include "Test.h"

extern "C"
{
    //------------------------Modèle linéaire----------------------------------------
    __declspec(dllimport) double* create_linear_model(int inputs_count);
    __declspec(dllimport) double predict_linear_model_classification(double* model, double inputs[], int sample_count);
    __declspec(dllimport) void train_linear_model_Rosenblatt(double* model, double all_samples[], int sample_count, int input_count,
        double all_expected_outputs[], int epochs, double learning_rate);
    __declspec(dllimport) void delete_linear_model(double* model);
}

int main()
{
    std::cout.precision(5);
    getchar();

    CasTest test(TestType::LinearSimple);
    test.DisplayInfos();

    double* model = create_linear_model(2);    

    std::cout << "BEFORE !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i ++)
    {
        std::cout << predict_linear_model_classification(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, 2) << std::endl;
    }

    train_linear_model_Rosenblatt(model, test.samples, test.sample_count, 2, test.outputs, 1000, 0.1);

    std::cout << "AFTER !" << std::endl;
    for (size_t i = 0; i < test.sample_count; i++)
    {
        std::cout << predict_linear_model_classification(model, new double[2]{ test.samples[i * 2], test.samples[i * 2 + 1] }, 2) << std::endl;
    }
    
    delete_linear_model(model);
}
