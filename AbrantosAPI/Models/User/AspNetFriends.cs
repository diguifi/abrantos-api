namespace AbrantosAPI.Models.User
{
    public class AspNetFriends
    {
            public int Id { get; set; }
            public bool IsConfirmed { get; set; }
            public virtual User FriendFrom { get; set; }
            public virtual User FriendTo { get; set; }

            public AspNetFriends()
            {
            }

            public AspNetFriends(User friendFrom, User friendTo)
            {
                FriendFrom = friendFrom;
                FriendTo = friendTo;
                IsConfirmed = false;
            }
    }
}