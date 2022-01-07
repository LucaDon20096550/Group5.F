using System;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime DateTimeSent { get; set; }
    public ApplicationUser Sender { get; set; }
    public Chat Chat { get; set; }
}