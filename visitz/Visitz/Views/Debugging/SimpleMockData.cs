using VisitzModel.Models.InPersonVisits;
using VisitzModel.Resources.Localization;

namespace Visitz.Views.Debugging;

internal static class SimpleMockData
{
    public static List<PersonVisit> MockPersonVisits(string parentId = "1-0000000")
    {
        List<PersonVisit> visits =
        [
            new()
            {
                Id = "1-1A2B3C4",
                ParentId = parentId,
                Name = "1-1A2B3C4",
                VisitDescription =
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                DateOfVisit = DateTimeOffset.UtcNow.AddDays(-14),
                LoginName = "SOMEUSER",
                Created = DateTimeOffset.UtcNow.AddDays(-14),
                Updated = DateTimeOffset.UtcNow.AddDays(-7),
                CreatedBy = "SOMEUSER",
                UpdatedBy = "SOMEUSER",
            },
            new()
            {
                Id = "1-1A2B3C5",
                ParentId = parentId,
                Name = "1-1A2B3C5",
                VisitDescription =
                    "Space, the final frontier. These are the voyages of the Starship Enterprise. Its five-year mission: to explore strange new worlds, to seek out new life and new civilizations, to boldly go where no man has gone before. Many say exploration is part of our destiny, but it’s actually our duty to future generations and their quest to ensure the survival of the human species.",
                DateOfVisit = DateTimeOffset.UtcNow.AddDays(-24),
                LoginName = "SOMEUSER",
                Created = DateTimeOffset.UtcNow.AddDays(-24),
                Updated = DateTimeOffset.UtcNow.AddDays(-2),
                CreatedBy = "SOMEUSER",
                UpdatedBy = "SOMEUSER",
            },
            new()
            {
                Id = "1-1A2B3C6",
                ParentId = parentId,
                Name = "1-1A2B3C6",
                VisitDescription =
                    "Put a record on and see who dances we're ahead of the curve on that one we need to dialog around your choice of work attire draft policy ppml proposal, or player-coach, or identify pain points build on a culture of contribution and inclusion. Overcome key issues to meet key milestones. I have a hard stop in an hour and half. Get buy-in who's responsible for the ask for this request?, but it's a simple lift and shift job. Take five, punch the tree, and come back in here with a clear head create spaces to explore whatâ€™s next teams were able to drive adoption and awareness, but golden goose. Shoot me an email we need to button up our approach, and we need to follow protocol what about scaling components to a global audience?, and flesh that out. Service as core &innovations as power makes our brand marginalised key performance indicators onward and upward, productize the deliverables and focus on the bottom line, but level the playing field, so we need to get all stakeholders up to speed and in the right place gage [sic] where the industry is heading and give back to the community what weâ€™ve learned. Can you put it into a banner that is not alarming, but eye catching and not too giant dog and pony show put a record on and see who dances, for blue sky, so we need to have a Come to Jesus meeting with Phil about his attitude, for we need to socialize the comms with the wider stakeholder community, for land it in region. Dunder mifflin touch base, but products need full resourcing and support from a cross-functional team in order to be built, maintained, and evolved, for powerPointless, yet iâ€™ve been doing some research this morning and we need to better, but zeitgeist, nor win-win. Currying favour action item, but gain traction powerpoint Bunny guerrilla marketing, so weâ€™re all in this together, even if our businesses function differently. Gain traction sacred cow, or upstream selling. 4-blocker value-added.\n\nCloser to the metal that's not on the roadmap are we in agreeance. Scope creep. Future-proof work flows red flag, but innovation is hot right now, drink from the firehose marginalised key performance indicators, in this space. Tread it daily helicopter view make it look like digital. Marketing, illustration wheelhouse.\n\nGoalposts. What the let's pressure test this move the needle, for usabiltiy, or let's put a pin in that. Create spaces to explore whatâ€™s next hop on the bandwagon, yet dear hiring manager:, so digitalize. Low hanging fruit blue money deploy, so 360 degree content marketing pool. Deploy. The horse is out of the barn c-suite come up with something buzzworthy you better eat a reality sandwich before you walk back in that boardroom, yet create spaces to explore whatâ€™s next wiggle room. Dear hiring manager: re-inventing the wheel, or are there any leftovers in the kitchen?, so baseline a better understanding of usage can aid in prioritizing future efforts we want to see more charts. Product launch can you run this by clearance? hot johnny coming through granularity, for ensure to follow requirements when developing solutions good optics, and talk to the slides. Customer centric we need to crystallize a plan roll back strategy that ipo will be a game-changer get in the driver's seat. Cloud native container based let's schedule a standup during the sprint to review our kpis. Copy and paste from stack overflow that's not on the roadmap circle back, root-and-branch review, for nail it down, goalposts. In this space marketing computer development html roi feedback team website, for that's mint, well done. Player-coach deploy to production target rich environment.",
                DateOfVisit = DateTimeOffset.UtcNow.AddDays(-14),
                LoginName = "SOMEUSER",
                Created = DateTimeOffset.UtcNow.AddDays(-14),
                Updated = DateTimeOffset.UtcNow.AddDays(-7),
                CreatedBy = "SOMEUSER",
                UpdatedBy = "SOMEUSER",
            },
            new()
            {
                Id = "1-1A2B3C7",
                ParentId = parentId,
                Name = "1-1A2B3C7",
                VisitDescription =
                    "That's great, but can you make it work for ie 2 please we are a non-profit organization theres all this spanish text on my site submit your meaningless business jargon to be used on the site!. We have big contacts we will promote you we are a startup, so can you make it faster?, for I got your invoice...it seems really high, why did you charge so much. Can you make the font bigger? something summery; colourful. Do less with more I know somebody who can do this for a reasonable cost can we have another option, yet can you make the blue bluer? the animation does not work, when i print the page. Jazz it up a little could you do an actual logo instead of a font we are a startup, but we are a non-profit organization, and it's great, can you add a beard though could you solutionize that for me i need this to work in internet explorer!. The animation does not work, when i print the page could you rotate the picture to show the other side of the room?, or i'll know it when i see it, but I have printed it out, but the animated gif is not moving, for this turned out different that i decscribed. Just do what you think. I trust you we are a big name to have in your portfolio, nor that's great, but can you make it work for ie 2 please.\n\nIt needs to be the same, but totally different i love it, but can you invert all colors?. I know you've made thirty iterations but can we go back to the first one that was the best version can it be more retro, for can my website be in english? we are your relatives, and could you move it a tad to the left, and the flier should feel like a warm handshake. Other agencies charge much lesser. I know this is the final release but can we add more features? can you make pink a little more pinkish, and try a more powerful colour can you rework to make the pizza look more delicious the website doesn't have the theme i was going for. Will royalties in the company do instead of cash can you turn it around in photoshop so we can see more of the front remember, everything is the same or better, for it looks a bit empty, try to make everything bigger, or anyway, you are the designer, you know what to do there is too much white space that's great, but can you make it work for ie 2 please. Can you make it look more designed I think we need to start from scratch. I need this to work in internet explorer! I think we need to start from scratch we don't need a backup, it never goes down! try a more powerful colour, so this looks perfect. Just Photoshop out the dog, add a baby, and make the curtains blue, and make the font bigger. This turned out different that i decscribed. Can we have another option will royalties in the company do instead of cash we need more images of groups of people having non-specific types of fun im not sure, try something else.\n\nGive us a complimentary logo along with the website can you make the logo bigger yes bigger bigger still the logo is too big, nor can you make pink a little more pinkish, nor anyway, you are the designer, you know what to do. Thanks for taking the time to make the website, but i already made it in wix can you make it stand out more? can you make it pop. Can you make it look more designed this is just a 5 minutes job, I think we need to start from scratch concept is bang on, but can we look at a better execution. This turned out different that i decscribed can it be more retro can you make the blue bluer?, nor doing some work for us \"pro bono\" will really add to your portfolio i promise, and i know this is the final release but can we add more features? you can get my logo from facebook. We need more images of groups of people having non-specific types of fun you might wanna give it another shot. Can you make the font bigger?. We are a startup can you rework to make the pizza look more delicious, nor can you make it faster?, yet i need this to work in internet explorer!. The target audience is makes and famles aged zero and up use a kpop logo that's not a kpop logo! ugh. Can you put \"find us on facebook\" by the facebook logo? can we have another option, nor is this the best we can do, for can you make pink a little more pinkish i think this should be fairly easy so if you just want to have a look. We don't need a contract, do we can you please send me the design specs again?, so can you make it pop use a kpop logo that's not a kpop logo! ugh, for I really like the colour but can you change it, so I need a website. How much will it cost. Can't you just take a picture from the internet?. ",
                DateOfVisit = DateTimeOffset.UtcNow.AddDays(-14),
                LoginName = "SOMEUSER",
                Created = DateTimeOffset.UtcNow.AddDays(-14),
                Updated = DateTimeOffset.UtcNow.AddDays(-7),
                CreatedBy = "SOMEUSER",
                UpdatedBy = "SOMEUSER",
            },
            new()
            {
                Id = "1-1A2B3C8",
                ParentId = parentId,
                Name = "1-1A2B3C8",
                VisitDescription =
                    "Cross functional teams enable out of the box brainstorming both the angel on my left shoulder and the devil on my right are eager to go to the next board meeting and say weâ€™re ditching the business model, but take five, punch the tree, and come back in here with a clear head. We need to make the new version clean and sexy. Drive awareness to increase engagement a loss a day will keep you focus. We have to leverage up the messaging sorry i didn't get your email. Incentivize adoption. We need distributors to evangelize the new line to local markets. Wheelhouse window of opportunity cannibalize pull in ten extra bodies to help roll the tortoise, and put in in a deck for our standup today we need distributors to evangelize the new line to local markets. Guerrilla marketing player-coach at the end of the day. Put your feelers out this is meaningless table the discussion , so enough to wash your face, yet we can't hear you .\n\nBaseline this is not the hill i want to die on, horsehead offer. Get in the driver's seat move the needle, nor baseline the procedure and samepage your department five-year strategic plan, and thought shower marginalised key performance indicators low engagement. Optimize the fireball regroup low-hanging fruit, nor circle back around circle back. In this space closing these latest prospects is like putting socks on an octopus, due diligence take five, punch the tree, and come back in here with a clear head, or prairie dogging that jerk from finance really threw me under the bus, or cta. Pivot do i have consent to record this meeting incentivization baseline the procedure and samepage your department, yet we need to touch base off-line before we fire the new ux experience, or can you ballpark the cost per unit for me. Let's prioritize the low-hanging fruit execute on-brand but completeley fresh, touch base. Please use \"solutionise\" instead of solution ideas! :) in an ideal world. Gain traction low-hanging fruit, cannibalize, for minimize backwards overflow. Into the weeds synergestic actionables weaponize the data new economy. We need to get all stakeholders up to speed and in the right place this is our north star design loop back we don't want to boil the ocean, nor when does this sunset? future-proof, for on your plate. Streamline going forward, but prethink, or i'm sorry i replied to your emails after only three weeks, but can the site go live tomorrow anyway?. Zoom meeting at 2:30 today turn the ship a tentative event rundown is attached for your reference, including other happenings on the day you are most welcome to join us beforehand for a light lunch we would also like to invite you to other activities on the day, including the interim and closing panel discussions on the intersection of businesses and social innovation, and on building a stronger social innovation eco-system respectively, and circle back, but we need to harvest synergy effects, yet talk to the slides, or guerrilla marketing. Drop-dead date if you want to motivate these clowns, try less carrot and more stick. We want to see more charts a better understanding of usage can aid in prioritizing future efforts talk to the slides let's see if we can dovetail these two projects, for this is a no-brainer. Driving the initiative forward form without content style without meaning. Currying favour we need to aspirationalise our offerings due diligence. We don't want to boil the ocean sea change. We need a paradigm shift sacred cow, and let's put a pin in that, yet green technology and climate change , or we need to get all stakeholders up to speed and in the right place, and iâ€™ve been doing some research this morning and we need to better. Work.\n\nCannibalize I just wanted to give you a heads-up. Future-proof slow-walk our commitment, flesh that out, yet root-and-branch review, nor can you put it on my calendar? draw a line in the sand increase the resolution, scale it up we need a larger print. Regroup work flows . This is not a video game, this is a meeting! we need to have a Come to Jesus meeting with Phil about his attitude, nor we don't need to boil the ocean here out of scope, dog and pony show. Run it up the flagpole, ping the boss and circle back pipeline, nor quarterly sales are at an all-time low are there any leftovers in the kitchen?. Drive awareness to increase engagement. Everyone thinks the soup tastes better after theyâ€™ve in it into the weeds, tribal knowledge can you slack it to me? my supervisor didn't like the latest revision you gave me can you switch back to the first revision? we've got to manage that low hanging fruit. Sea change pig in a python, but we need a paradigm shift, for build on a culture of contribution and inclusion, yet staff engagement. Make it more corporate please locked and loaded spinning our wheels.",
                DateOfVisit = DateTimeOffset.UtcNow.AddDays(-14),
                LoginName = "SOMEUSER",
                Created = DateTimeOffset.UtcNow.AddDays(-14),
                Updated = DateTimeOffset.UtcNow.AddDays(-7),
                CreatedBy = "SOMEUSER",
                UpdatedBy = "SOMEUSER",
            },
        ];

        visits.ElementAt(0).VisitDetails.Add(PersonVisitDetails.Api_NotPrivatePlanning);
        visits.ElementAt(1).VisitDetails.Add(PersonVisitDetails.Api_PrivateVisitNotInHome);
        visits.ElementAt(2).VisitDetails.Add(PersonVisitDetails.Api_ExemptionChildDeclined);
        visits.ElementAt(3).VisitDetails.Add(PersonVisitDetails.Api_PrivateVisitMedicalSupportNeeds);
        visits.ElementAt(4).VisitDetails.Add(PersonVisitDetails.Api_NotPrivateWithCaregiver);

        return visits;
    }
}
