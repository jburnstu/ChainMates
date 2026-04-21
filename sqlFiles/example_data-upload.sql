call chain_mates.empty_database();
call chain_mates.populate_example_data();

create or replace procedure chain_mates.populate_example_data() 
language sql as 
$procedure$
insert into chain_mates.author (id,display_name,email_address,"password", date_created)
values
(1, 'Shakespeare3000', 'Shakespeare3000@example.com', 'Shakespeare3000', clock_timestamp()),
(2, 'Woolf99', 'Woolf99@example.com', 'Woolf99', clock_timestamp()),
(3, 'Shelley55', 'Shelley55@example.com', 'Shelley55', clock_timestamp()),
(4, 'Eliot1', 'Eliot1@example.com', 'Eliot1', clock_timestamp()),
(5, 'Murdoch111', 'Murdoch111@example.com', 'Murdoch111', clock_timestamp()),
(6, 'Marlowe500', 'Marlowe500@example.com', 'Marlowe500', clock_timestamp()),
(7, 'Chaucer28', 'Chaucer28@example.com', 'Chaucer28', clock_timestamp()),
(8, 'Zola123', 'Zola123@example.com', 'Zola123', clock_timestamp()),
(9, 'Hegel900', 'Hegel900@example.com', 'Hegel900',clock_timestamp()),
(10, 'Asimov40', 'Asimov40@example.com', 'Asimov40',clock_timestamp()),
(11, 'Verne7', 'Verne7@example.com', 'Verne7',clock_timestamp())
;
insert into chain_mates.story (id, author_id, title, date_created)
values
(1, 1, 'The Storm', clock_timestamp()),
(2, 2, 'The Ship', clock_timestamp()),
(3, 3, 'The Journey', clock_timestamp()),
(4, 1, 'The Old House', clock_timestamp()),
(5, 4, 'The Stranger', clock_timestamp()),
(6, 8, 'The City', clock_timestamp())
;
insert into chain_mates.segment (id, author_id, story_id, content, segment_status_id, previous_segment_id, date_created)
values 
(1, 1, 1, $$It was a stormy night.$$, 5, null, clock_timestamp()),
(2, 2, 2, $$Somewhere out to sea, past the stone harbour walls, a bell was ringing.$$, 4, null, clock_timestamp()),
(3, 3, 3, $$Soon all the goodbyes were done, and it was time to leave.$$, 4, null, clock_timestamp()),
(4, 1, 4, $$No one ever goes up there these days. Parents forbid their children from even crossing the brook at the foot of the property. Teenagers dare each other up for the night, but most don't even make it to the outhouse before finding a way to laugh the whole thing off and go home.$$, 4, null, clock_timestamp()),
(5, 1, 2, $$It was an indescribably mournful noise. The ring of the bell was suffocated to a choking thud by the fog, its pitch dropping each time as though losing the strength to finish. $$, 4, 2, clock_timestamp()),
(6, 4, 1, $$Nobody could sleep.$$, 4, 1, clock_timestamp()),
(7, 4, 5, $$In some parts of the world, it's customary to welcome a travelling stranger in for a meal when they get to town. Folks will fight for the honour of wthergfefrgae. This was not one of those parts of the world.$$, 5, null, clock_timestamp()),
(8, 9, 3, $$Their mother put it off as long as she could, fussing over their hat and their cloak as though the hundredth inspection would reveal something the others hadn't. Their younger brother tugged at their coat, asking with the sullen guile of the very young whether they would read him one more story before leaving. Eventually, they had to walk backwards out of the door just to fend off all the hands.$$, 4, 3, clock_timestamp()),
(9, 5, 3, $$The road outside seemed unnaturally still in the twilight. After a second, they felt the slight pricking of the wind on the hair of their arms, which were exposed in their summer shirt -- the coolest piece of clothing they were taking with them. They took one last look at the doorway filled with the sad, mournful faces of their family, waved, and started off towards the setting sun.$$, 5, 8, clock_timestamp()),
(10, 4, 2, $$Dana shook herself, realising that she'd been stood facing the harbour for some time, listening to that noise. She wasn't the only one. More than a few of the figures on the street ahead of her had paused in whatever business had brought them out before dawn to face the same way. She passed a woman, well-dressed, with a few bolts of cloth held limply against her side. Her face was marked with... pain, almost -- a look of affliction, of deep hurt. As Dana caught her eye, she came to. They shared a moment of confusion, before she laughed awkwardly and hurried off with a murmured apology. Dana was pretty sure it wasn't the way she had been going.$$, 5, 5, clock_timestamp()),
(11, 8, 4, $$Not that I was any different bck when I was a  boy. Frankly, I wasn't the kind of kid who had to be forbidden from going up to Highborough, nor the kind who would humour a dare sending him up there (not that I had many of the kind of friends who would even try). $$, 4, 4, clock_timestamp()),
(12, 5, 5, $$This was the kind of town where stopping too long by an open door was as likely to get you a warning shot as a welcome. Where being out or doors after midday was a provocation. And after sundown, a threat.$$, 5, 7, clock_timestamp()),
(13, 10, 2, $$Jon listened to it for a minute, feet up on the desk of the boatmaster's office. How had such a quiet noise managed to wake him up? He deliberated briefly whether to ask his boss what to do. He'd just about decided to try and get back to his nap, when the housting from outside started. He looked out the window to see a growing crowd moving towards the harbour wall.$$, 4, 5, clock_timestamp()),
(14, 6, 4, $$So what the hell am I doing back here? I glance behind me at the fence, still fallen down in the same place I'd overheard my classmates talkign about all those years ago, and never seen. In my pocket, my fingers rhythmically flatten out and re-crumple an old scrap of paper. The stray cutting of newspaper that brought  me here.$$, 4, 11, clock_timestamp()),
(15, 11, 2, $$Nobody in the city heard it, even as it rang and rang. The usually inquisitive nightgulls feigned disinterest. As the stars dove on across the sky, finally a fisherwoman looked up. She was bone-weary from the day, and not long from home. But, as she listened, her curiosity and concern for whoever might be out there won out. And the sea was calm, she told herself.$$, 5, 2, clock_timestamp()),
(16, 9, 4, $$It's probably not surprising$$, 1, 4, clock_timestamp()),
(17, 7, 2, $$There was hardly a beast in the sky or fish in the sea to note her passing. Just the patter of the water against her hull, forming a gentle, repetitive rhythm with thte bell and the high whistle of the wind over open water. She almost thought she might have fallen into a dream when she first saw the shape of the ship looming ahead of her.$$, 2, 15, clock_timestamp()),
(18, 7, 4, $$There's no choice to be made here. And no way back through that fence without finding out what I caem here for. That's what I tell myself, anyway. It takes a push to get myself startd up that hill.$$, 1, 14, clock_timestamp()),
(19, 7, 2, $$With each street closer to the sea-front, the crowds thickened, and so did the effect of the bell.$$, 4, 10, clock_timestamp()),
(20, 4, 4, $$Some of that's probably the centuries of rumours of hauntings. Some of it might be that strong of murders in the 60's. But honestly, even without all that, the place just has that look: "Bad Things Happen Here". Even a shit-talking teen knows when a place has that look.$$, 5, 4, clock_timestamp()),
(21, 8, 6, $$Once upon a time, there was a city made completely of jelly.$$, 2, null, clock_timestamp()),
(22, 5, 2, $$She hurried back etc etc etc$$, 1, 10, clock_timestamp()),
(23, 6, 1, $$The wind was too loud.$$, 4, 6, clock_timestamp()),
(24, 10, 1, $$The whole town was fast asleep.$$, 2, 1, clock_timestamp()),
(25, 5, 4, $$Deleted story$$, 5, 20, clock_timestamp()),
(26, 4, 3, $$Their family watched in the doorway as thier sillhouette wobbled into the light, eventually becoming invisible against the sun's last creamy red light. When it finally set completely, they had gone.$$, 2, 9, clock_timestamp()),
(27, 1, 5, $$This was the moon.$$, 2, 7, clock_timestamp()),
(28, 11, 4, $$Deleted$$, 4, 9, clock_timestamp()),
(29, 11, 5, $$It must have been past midnight when Sally woke up to the sound of  hooves against the Main Street cobbles -- the only road in town that wasn't dirt. She lay there in bed for a minute, then practically flung herself over to the window at the foot of her bed. Whatever was about to happen, she knew she was going to want to see it. That was the pain about a town as unfriendly as this: when everyone knew the rules, things got boring.$$, 2, 12, clock_timestamp()),
(30, 10, 4, $$In progress$$, 1, 20, clock_timestamp())
;
$procedure$;