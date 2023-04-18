#include<iostream>
#include"omp.h"

using namespace std;

const int threads_count = 16;
const int rows_count = 20000;
const int columns_count = 20000;
int arr[rows_count][columns_count];

void init_arr();
void get_sum();
void get_min_row_sum();

int main() {
	
	omp_set_nested(1);
	omp_set_num_threads(threads_count);
	cout << "\nthreads count : " << threads_count << endl;
	init_arr();
	double t1 = omp_get_wtime();
	
#pragma omp parallel sections
	{
#pragma omp section
		{
			get_sum();
		}
#pragma omp section
		{
			get_min_row_sum();
		}
	}
	
	double t2 = omp_get_wtime();
	cout << "\nTotal time - " << t2 - t1 << " seconds" << endl;
	return 0;
}

void init_arr() {
	double t1 = omp_get_wtime();
#pragma omp parallel for
	for (int i = 0; i < rows_count; i++)
	{
		for (int j = 0; j < columns_count; j++)
		{
			arr[i][j] = (rows_count * columns_count) - (i * j);
		}
	}
	arr[rows_count / 2][columns_count / 2] = -100;

	double t2 = omp_get_wtime();
	cout << "\nInit array, threads worked - " << t2 - t1 << " seconds" << endl;
}

void get_sum() {
	long long sum = 0;
	double t1 = omp_get_wtime();
	
#pragma omp parallel for reduction(+:sum) collapse(2)	
		for (int i = 0; i < rows_count; i++) {			
			for (int j = 0; j < columns_count; j++)
			{
				sum += arr[i][j];
			}
		}

	double t2 = omp_get_wtime();
	cout << "\nTotal sum = " << sum << ", threads worked - " << t2 - t1 << " seconds" << endl;
}

void get_min_row_sum() {
	int row_min_index;
	long long min = 10000000000000;
	long long min_rows[rows_count];
	for (int i = 0; i < rows_count; i++)
	{
		min_rows[i] = 0;
	}
	
	double t1 = omp_get_wtime();
	
#pragma omp parallel for
	for (int i = 0; i < rows_count; i++) {
		for (int j = 0; j < columns_count; j++)
		{
			min_rows[i] += arr[i][j];
		}
		if (min > min_rows[i])
		{
#pragma omp critical
			if (min > min_rows[i])
			{
				min = min_rows[i];
				row_min_index = i;
			}
		}
	}

	double t2 = omp_get_wtime();
	cout << "\nMin row " << row_min_index << " with sum " << min << ", threads worked - " << t2 - t1 << " seconds" << endl;
}