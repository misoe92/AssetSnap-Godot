namespace AssetSnap.Nodes
{
	using AssetSnap.Front.Nodes;
	using Godot;

	public class ModelDriver
	{
		public enum ModelTypes 
		{
			Simple,
			SceneBased,
		};
		
		public ModelTypes ModelType { get; set;}
		
		/*
		** Goes through the node structure to decide
		** what type of model we are working with
		**
		** @param Node3D node
		** @return void
		*/
		public void RegisterType( Node3D node ) 
		{
			if( node is AsMeshInstance3D ) 
			{
				ModelType = ModelTypes.Simple;
			}
			
			if( node is AsNode3D ) 
			{
				ModelType = ModelTypes.SceneBased;
			}
		}
		
		/*
		** Returns the defined model type
		**
		** @return ModelTypes
		*/
		public ModelTypes GetModelType()
		{
			return ModelType;
		}
	}
}