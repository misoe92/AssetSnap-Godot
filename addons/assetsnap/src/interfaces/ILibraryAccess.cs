namespace AssetSnap.Interfaces
{
	public interface ILibraryAccess
	{
		void SetLibrary(Library.Instance _LibraryInstance);
		Library.Instance GetLibrary();
	}
}
