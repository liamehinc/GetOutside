using GetOutside.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GetOutside.Database
{
    public interface iLocalDataService
    {
        void Initialize();
        void CreateOutsideActivity(outsideActivity outsideActivity);
        System.Collections.Generic.List<outsideActivity> GetOutsideActivity();
        void DeleteOutsideActivity(outsideActivity outsideActivity);
        void UpdateOutsideActivity(outsideActivity outsideActivity);

    }
}