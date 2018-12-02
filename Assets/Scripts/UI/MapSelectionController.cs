using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelectionController : MonoBehaviour
{
    public Text MapName;
    public Image MapImage;
    public GameObject UpArrow;
    public GameObject DownArrow;

    private PlayerInputManager _inputManager;
    private int _mapCount = MapManager.MapList.Count;
    private int _loadedMapID = -1; // nothing loaded yet
    private int _selectedMapID = 0;
    private bool _isAbleToScroll = true;


    void Start ()
    {
        _inputManager = GetComponent<PlayerInputManager>();
    }

    void Update ()
    {
        if (TryLoadMapData(_selectedMapID))
            return;

        foreach (var player in Globals.Players)
        {
            _inputManager.PlayerId = player.ControlsId;

            // ! Starts the game
            if (_inputManager.IsButtonPressed(PlayerInputManager.Key.Jump)) {
                var test = MapManager.MapList;
                SceneManager.LoadScene(_selectedMapID + 1);
            }

            // Moved down the list.
            if (_isAbleToScroll && _inputManager.GetAxis(PlayerInputManager.Key.MoveVertical) < 0)
            {
                _isAbleToScroll = false;

                if (_selectedMapID < _mapCount - 1)
                    _selectedMapID++;
            }

            // Moved up the list.
            else if (_isAbleToScroll && _inputManager.GetAxis(PlayerInputManager.Key.MoveVertical) > 0)
            {
                _isAbleToScroll = false;

                if (_selectedMapID > 0)
                    _selectedMapID--;
            }

            // Released button.
            else if (_inputManager.GetAxis(PlayerInputManager.Key.MoveVertical) == 0)
                _isAbleToScroll = true;
            }
    }

    /// <summary>
    ///     Enable/Disabled choosing arrows if reached first/last possible map.
    /// </summary>
    public void UpdateArrows ()
    {
        UpArrow.SetActive(_selectedMapID > 0);
        DownArrow.SetActive(_selectedMapID < MapManager.MapList.Count - 1);
    }

    private bool TryLoadMapData (int mapID)
    {
        if (mapID == _loadedMapID)
            return false;
        else
            _loadedMapID = mapID;

        // Get the map to load.
        var map = MapManager.MapList[mapID];

        // Load map preview image as a texture.
        var mapTexture = Resources.Load(map.PreviewResource) as Texture2D;
        // Convert loaded texture to a sprite and set it to be displayed in the UI.
        MapImage.sprite = Sprite.Create(mapTexture, new Rect(0f, 0f, mapTexture.width, mapTexture.height), new Vector2());

        MapName.text = map.Name; // display map name in the UI

        UpdateArrows();
        return true;
    }
}