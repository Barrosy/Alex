using Alex.MoLang.Parser;
using Alex.MoLang.Runtime;
using Alex.ResourcePackLib.Json.Bedrock.Entity;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace Alex.ResourcePackLib.Json.Bedrock.Particles.Components
{
	public class MotionComponent : ParticleComponent
	{
		[JsonProperty("linear_acceleration")]
		public MoLangVector3Expression LinearAcceleration { get; set; }
		
		[JsonProperty("linear_drag_coefficient")]
		public IExpression[] LinearDragCoEfficientExpressions { get; set; }
		
		public float LinearDragCoEfficient(MoLangRuntime runtime) => runtime.Execute(LinearDragCoEfficientExpressions).AsFloat();
	}
}