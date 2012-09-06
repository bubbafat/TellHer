namespace TellHer.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<TellHer.Data.TellHerDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(TellHer.Data.TellHerDb context)
        {
            List<string> ideas = new List<string> {
                "Your first words of the morning matter.  Don\'t grunt and fart.  Instead, try \"good morning, beautiful.\"",
                "Get up a few minutes early and surprise her with a quick breakfast in bed.  Those 5 minutes will stay with her all day.",
                "Spend 10 minutes cleaning your bedroom tonight.  She will be able to relax a lot more with less clutter in sight.",
                "Tonight you make dinner – and afterwards you clean the dishes.  No kids?  Do them naked.",
                "Come home 15 minutes early and use the time to clean the house.",
                "Give her a night off from you and the kids.  Take them out so she can spend some time on a hobby or just relaxing.",
                "Invite her on a walk.  Hold her hand.  Ask about her day.  Listen to her.",
                "When she is telling you about a problem don\'t try to solve it.  Listen then ask if you can help.  Believe her answer.",
                "Have 15 minutes to kill?  Clean the bathroom – including the toilet.",
                "Out of shape?  Take care of yourself.  Make regular exercise a priority.  You might even invite her to join you.",
                "Knock something off the honey-do list without being asked.  Find a 15 minute job and do it.  You have the time.",
                "Find something to compliment her about daily and then actually say it.",
                "Notice her hair today.  She put effort into making it right.  She did that for you – appreciate it!",
                "Notice her eyes today.  Tell her how beautiful they look.",
                "Notice her smile today.  Tell her how it makes you feel to see her happy.",
                "Going out?  Compliment what she is wearing.  She picked it as much for you as for her.  Let her know you noticed.",
                "When she gets a new outfit be sure to notice it.  Clueless?  Look for hints (bags, receipts, etc).",
                "If you know she went to the hair salon then for the love of all that is holy you had better compliment her hair.",
                "Leave a simple love note in her purse when you leave in the morning.  You\'ll know when she finds it.",
                "Leaving early?  Put a little a love note on the bathroom counter for her to find later.",
                "Every once in a while send her a text message saying you miss her.  Not every day, though.  Make it special, not routine.",
                "Is she having a rough day?  Send her an encouraging text or email.",
                "Leave a note for her on her steering wheel.",
                "Write her an honest-to-god love poem.  Long, short, rhymes or not ... doesn\'t matter.",
                "Traveling apart?  Send her a postcard with a note saying how much you miss her.",
                "Tell her you love her.  Right now.  Take her hand, look her in the eyes and say it!  Now!",
                "Tell her how happy you are to have married her.  Don\'t assume she knows it.  Let her hear it.",
                "Tell her that you missed her today and how happy you are to be with her now.",
                "When she accomplishes something make sure to actually say \"I\'m proud of you\"",
                "Tell her that you appreciate her and what she does for you and your family.",
                "Stop calling her \"dude\" and \"man\".  Just stop it.",
                "Meet her at the door once in a while.  Make her feel like you were waiting just for her to come home.",
                "Hold her hand in public.  Don\'t wait for her to grab yours.",
                "Hands full?  Stick out your elbow and let her lock arms with you.  Let her know that her touch matters to you.",
                "When sitting at a table let those feet wander a bit.  Play footsie with her.",
                "Walking single file?  Put your hand gently on the small of her back.  Let her know you\'re with her.",
                "Don\'t be afraid to give her a little kiss in public.  She\'s beautiful.  You\'re proud of her.  Let everyone know it.",
                "Be a gentleman.  Always let her sit down first.",
                "Be a gentleman.  On your next date why not open the car door for her?",
                "Be a gentleman.  Open the door for her.  Little gestures go a long way.",
                "Put your arm around her waist when you are walking in public.",
                "Does she have a pinterest board?  Check out her pins every once in a while.  Learn something about her interests.",
                "Does she have a hobby you don\'t understand?  Ask her to teach you.",
                "Make her a mixtape.  Spend 20 minutes thinking about songs that make you think of her.  Make sure \"your\" song is on it.",
                "When she says her feet are killing her that\'s your cue to offer a foot rub.",
                "Sitting on the couch together?  Offer her a neck rub.",
                "Man Tip: Spend 15 minutes learning about back and neck massage online and remember to always start gently.",
                "Sometimes it\'s less about where you touch as how you touch.  Ask her what she likes.",
                "It\'s much more enjoyable to be touched by someone with groomed nails.  File those bad boys a little.",
                "When was the last time you walked up behind her, put your arms around her and hugged her?",
                "Find somewhere new to kiss her.  No ... I don\'t mean there.  How about her knee?  Elbow?  Hip?  Have a little fun.",
                "Cuddling is not always a prelude to sex.  Do it without expectation and you may find it leads there more often.",
                "Make tonight a movie-in-bed date night.  Ask her out.  Let her pick the movie.  You make a snack.  Relax and unwind.",
                "Ask her out for a picnic.  Check the weather, pick a location, pack the meal – all she has to do it show up and enjoy.",
                "Take her hiking.  It doesn\'t have to be extreme.  A 1 or 2 mile walk around a lake is a great way to unwind.",
                "When was the last time you asked her out on a date as if you were still a new couple?  She misses that.",
                "Is there a movie she wants to see but you don\'t?  Take her anyway.  She\'ll appreciate it more than you think.",
                "Take her out for karaoke and sing her a love song.  Super cheesy but she\'ll remember it for a long time.",
                "Surprise her with a simple bouquet of her favorite flowers.  (You know her favorite flower, right?)",
                "Stop and pick up a card for no reason at all.  Make it a little funny and a little sappy.  Just let her know you care.",
            };

            ideas.Shuffle();

            foreach (string ideaStr in ideas)
            {
                DailyIdea idea = new DailyIdea
                {
                    Idea = ideaStr,
                };

                if (!context.DailyIdeas.Any(i => i.Idea == idea.Idea))
                {
                    context.DailyIdeas.AddOrUpdate(idea);
                }
            }
        }
    }

    internal static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
