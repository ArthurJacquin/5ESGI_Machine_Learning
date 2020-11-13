#pragma once
#include <cstdlib>

enum TestType {
	LinearSimple,
	LinearMultiple,
	XOR,
	Cross,
	MultiLinear,
	MultiCross,
	LinearSimple2D,
	NonLinearSimple2D,
	LinearSimple3D,
	LinearTricky3D,
	NonLinearSimple3D
};

struct CasTest {

	int sample_count;

	double* samples;
	double* outputs;

	CasTest(TestType t);

	void DisplayInfos();
};