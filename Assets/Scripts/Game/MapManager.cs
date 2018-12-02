using System.Collections.Generic;

public static class MapManager
{
    private static IList<IMapDetails> _maps = new List<IMapDetails>();
    public static IList<IMapDetails> MapList
    {
        get { return _maps; }
    }


    static MapManager ()
    {
        _maps.Add(new GardenOfTruthDetails());
        _maps.Add(new FinalValleyMapDetails());
    }
}