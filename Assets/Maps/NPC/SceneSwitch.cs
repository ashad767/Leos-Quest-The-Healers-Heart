using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneSwitch : MonoBehaviour

{
    public Player player;
    public GameObject playerCutsceneSprite;
    public Vector3 NextRoom;

    public string SceneToSwitchToo;

    public bool isSceneSwapper;
    public void CutSceneSceneSwitch()
    {
        player.transform.position = NextRoom;
        playerCutsceneSprite.transform.position = NextRoom;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!isSceneSwapper)
        {
            if (other.CompareTag("Player"))
            {

                player.transform.position = NextRoom;
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {

                SceneManager.LoadScene(SceneToSwitchToo);
            }
        }
    }
}
