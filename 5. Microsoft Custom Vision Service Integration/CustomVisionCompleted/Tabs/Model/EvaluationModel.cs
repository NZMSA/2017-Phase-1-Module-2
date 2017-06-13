using System;
using System.Collections.Generic;

namespace Tabs.Model
{
    public class EvaluationModel
    {
		public string Id { get; set; }
		public string Project { get; set; }
		public string Iteration { get; set; }
		public string Created { get; set; }
		public List<Prediction> Predictions { get; set; }
    }

	public class Prediction
	{
		public string TagId { get; set; }
		public string Tag { get; set; }
		public double Probability { get; set; }
	}
}
