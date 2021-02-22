using GetOutside.Core.Model;


namespace GetOutside.Database
{
    public interface IILocalDataService
    {
        void Initialize();
        void CreateOutsideActivity(OutsideActivity outsideActivity);
        System.Collections.Generic.List<OutsideActivity> GetOutsideActivity();
        void DeleteOutsideActivity(OutsideActivity outsideActivity);
        void UpdateOutsideActivity(OutsideActivity outsideActivity);

    }
}