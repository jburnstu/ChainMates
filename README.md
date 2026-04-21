# ChainMates (Working Title)

This is the development code-base for my personal project, ChainMates (one of many working titles) -- a **desktop (for now) app** on which users *create, write and submit stories*, which are then sent on for other users to continue and *branch off of*.<br><br>

This app started life in Django (see the repo "chainletters", from which I've copied over this ReadMe as a starting point), with the aim of keeping my Python / SQL skills sharp, and expand my knowledge of HTML / CSS / Javascript, in particular JS-React.

I've since changed over to ASP.NET in C#, which I worked with briefly in both my degree and most recent job. I've really enjoyed the change-over, and it's been fun a fun complement to a recent game-modding project in Java (also a repo on this account) but for something more professionally-aligned.

The tech I've used:
- **PostgreSQL** database (currently hosted on my own laptop, and managed via DBeaver) consisting of base tables and supporting views;
- Accessed via an **ASP.NET Core** Backend (written in **C#**), processed via services / controllers;
- Passed to a HTML / CSS / **Javascript-React** frontend (built with Vite).
- Whole thing managed in **GitHub** (obviously).

If you're here because I plugged this project in a job application, thank you for coming to have a look! In other projects I've done, I've found it a bit easier to point to one "thing" (one folder, one set of classes etc) that I felt "showcased" my work. That's a bit harder in this repo, because I really just wanted to show the ability to build an app overall. Also, to be honest, the bits I'm proudest of here are also works in progress (comments, notifications). So I'd suggest just skimming the file structure, and judging the project by how possible it is to parse what I've been doing. Feedback always welcome!

## General Coding Comments

### Object Structure
As a brief overview, the key objects handled by the app are Authors (users), Stories, Segments (parts of stories), Comments, and Notifications.
Besides Stories and Segments, the use-cases of these objects are fairly self-explanatory. 
The segment object is the site of most of the business logic in the app. Segments represent units of a story, each written by one author; hence, segments may follow one, and be followed by many, other segments (in a branching chain). They hold all of the actual "writing" done in the app.
The segment's life-cycle is currently managed by its SegmentStatusId field:
- 1 (inProgress): being written
- 2 (openForModeration): submitted, and not present in the frontend until a user picks it up to moderate it
- 3 (lockedForModeration): present on a user's "Review" tabm where it sits until they approve it to be published
- 4 (openForAddition): this segment is public on the website. It will turn up in searches when users go to join onto a story
- 5 (lockedForAddition): still public, but ensures thata segment may only have one segment being added to it at any time. (reverts to 4 when the segment being added is published.)
- 6 (abandoned): "Deleted" as far as the frontend knows.

Compare this to the user experience -- sorted by tab:
- Write tab: the user may
      - Start a new story
      - Pick from a random selection of available segments to join
      - Save, submit, and/or delete the segments they have written to start / join those stories.
- Review tab: the user may
      - Pick from a random selection of segments that require moderation
      - Approve the moderation of those segments ("publishing" them)
- Author / story search tabs: the user may:
      - Search for and read stories
      - View other author's profiles and optionally follow/unfollow them.
  
The Story object actually doesn't have a lot of logic attached to it (yet, eventually it will impose limits on segments eg word counts) -- it simply encpsulates all the segments which chain back to the same starting point. Most of the logic of what gets given /shown to the user, is handled by the segment object.

### Repository Structure
If you're here because of a link from a job application -- thank you for having a look at my code! I've been guided by Visual Studio's project / solution setup when structuring the repository. Most of the code of interest is in the following places:
- **ChainMates\chainmates.client\src\** holds most of the React code, which flows from the central *app.jsx* file. Most of the structure is evident there, from the routing to the higher-level state variables. This feeds into the:
    - *pages/* folder and *layouts/layouts.jsx* file, which control the various pages of the app,
    - *buttons/* and *updates/* folders, and *segmentDisplay.jsx* file, which handle sub-sections and widgets, and the
    - *utilityFuncs/* folder, which houses the login/signup-loop, and also some support functions (including an API access point).
-  **ChainMates\ChainMates.Server** houses all ther server code. This includes folders for Controllers, DTOs, and Services, split rouughly according to which set of Objects they concern: Author, Story, Segment, Comment, and Notification. In terms of interesting code to look at, some notes:
    - The top-level **Entities.cs** file is a good place to start for understanding the object structure underlaying the app. (Roughly, this goes Author -> Many Stories -> Each Have Many Segments -> All Have Many Comments -> All Prompt Many Notifications.)
    - Most of the logic happens in the **Segment** files, to the point where I'm considering splitting some of it out into a separate Rules folder (EG the methods to determine which segments an author can join review are currently in **SegmentService**).
    - The **Notification** Entity / table, and associated Service / DTOs, are a good example of  *json-column* usage (although I'm still building it out).
    - The *Comment* Entity / table, and the "inherited" tables (per type-of-object-commented), are a good example of *Class-Table Hierarchy* (or they will be when I get this to work in EFC... they might end up being json-ified as well).
- **ChainMates\ChainMates.Test\** is the project I've set up for testing. As yet, the app hasn't really been fully-formed enough for this kind of testing to apply, but I wanted to set it up to demonstrate capability (and I'll be extending it soon).

### Development History
The rough order in which this project came about:
1. In Django:
  a. Built an early version of the database to help conceptualise the objects involved
  b. Learnt Django via the online tutorial, synced to the existing database, and set up the app as a series of views and HTML templates
  c. Added the progress thus far to GitHub and added some basic Javascript / CSS
  d. Introduced React in a new git branch
  e. Shifted slowly from a classic website to SPA model, introducing DRF views for database access
Then in C#:
  a. Rebuilt the Database starting from EFC, to ensure compatibility
  b. Carried over the frontend directly, with minor config changes
  c. Transformed the server logic piecemeal from Python to C#. This was a lot of work, but actually the structure that C#/.NET encourages you towards is much more intuitive to me, so this was actually quite satisfying.
  d. Set up a new git repository from around here
  e. From here, added features "natively" to the C# version (I'm not updating gthe DJango version for now).

### Omissions and Oversights
This is a personal project and first and foremost a learning exercise, and as such I've prioritised trying things out and learning concepts over "business-ready" best practice. However, including/demonstrating use of some of these practices is my next priority. These include:
- **Testing**: While I have tested the code as I've gone, this hasn't been particularly rigorous. My next objective is to expand the unit-testing project (and look into some user-testing from my friends).
- **Authentification**: I've been learning Auth set-up for ths project, and while I used AI to generate some initial code and ideas, I've got enough of a grasp of it now that I'd like to take it to the next level purely based on my own understanding.
- **Code Comments**: I've favoured a "comments-lite" approach since it's just me working on this for now (a lot of the comments are more notes to myself), but at some point I will go through and ensure the code is sufficiently well-documented to be understood.
- **Optimisation**: This hasn't been a focus until the app is up-and-running, but I'm keen to look into any choke-points that have gone unnoticed under the current development conditions.

### Use of AI
In general I try to avoid using AI too much during personal projects, as I worry it will interrupt the learning process. That said, for this project, it is sometimes useful for sanity-checking structural decisions that can be difficult to search properly on, say, StackOverflow.
Explicit use is mentioned in the code comments on relevant files, but the main places:
- **Authentification**: I used AI to set up the simple authentification loop used by this project. This is because I wanted something present so I could plan around it when building the rest of the API layer, but I didn't want to get bogged down in understanding how C# specifically handles this. I've since gone back and understand the code it gave me (and I plan on replacing a lot of it before long).
    - This is the only place that I explicitly copied code from AI. in some places, Visual Studio now auto-completes surprisingly large blocks, which I haven't switched off but do find somewhere between mildly-time-saving and annoying.
- **Server issues and 502 errors**: When I set this project up, I *could not* get the out-of-the-box server set-up to work. It had me at my wit's end, so I asked chatGPT for help, which actually ended up making everything worse, which was a valuable lesson in what not to use AI for.
- **Architectural Questions**: To this point, I actually think vague questions to which you sort of know the answer are often what AI is best for. It's well-documented that, say, chatGPT is a complete yes-man, but it will respond sensibly to an A/B question (should I put this here or there?) in a way that can be a good starting place for further google searches, etc.. 


### Use of GitHub
In spite of this being a solo project, I've still made use of GitHub's branch feature. I've made a couple of abortive attempts to get into its "Issues" and other features, but I think these feel pretty untenable out of a group setting.<br><br>
At time of writing, I've been without wifi for a couple of weeks (new house) so the timings of git merges might look a bit strange -- basically I've been chaining branches on my local machine, and then merging them into master sequentially when I can get to a cafe.


