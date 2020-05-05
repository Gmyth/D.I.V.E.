using UnityEngine;


public class CollectibleItem : Item
{
    [SerializeField] uint achievementID;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Collect();
            AudioManager.Singleton.PlayOnce("Pick_cassette");
        }
            
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Collect();
            AudioManager.Singleton.PlayOnce("Pick_cassette");
        }
    }


    private void Collect()
    {
        Player.CurrentPlayer.AddCollectibleItem(itemID, achievementID);

        Destroy(gameObject);
    }


    private void Awake()
    {
        if (Player.CurrentPlayer.HasAchievement(achievementID))
            Destroy(gameObject);
    }
}
