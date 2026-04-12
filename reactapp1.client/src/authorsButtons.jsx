
import React, { useState, useEffect, useContext } from "react";
import { createPortal } from 'react-dom';
import { AuthorContext } from "./context.jsx";
import { useNavigate, useLocation, redirect } from "react-router";
import { getRandomItem, contactAPI } from "./utilityFuncs.jsx";


// export function ModalSelectFriendButton(props) {
//     return (<>
//         <button onClick={createModal}>{props.type}
//         </ button >
//         <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
//             <div className="allDisplayStoriesContainer">
//                 {arrayOfSe.map(availableStory =>
//                     <FriendListDisplayInModal key={availableStory.id} selectStory={selectStory} storyDict={availableStory} />
//                 )}
//             </div>
//         </ModalWindow >
//     </>
//     )
// }


// function SearchDisplayInModal(props) {

//     return (
//         <button onClick={selectStory} className="displayStoryContainer">
//             <textarea value={firstSegment.earlier_segment_content} readOnly />
//             {(finalSegment != null) ? <textarea value={finalSegment.earlier_segment_content} readOnly /> : null}
//         </button>
//     )
// }


// export function ModalAuthorSearchButton(props) {
//     return (<>
//         <button onClick={createModal}>{props.type}
//         </ button >
//         <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
//             <div className="allDisplayStoriesContainer">
//                 {arrayOfSe.map(availableStory =>
//                     <SearchDisplayInModal key={availableStory.id} selectStory={selectStory} storyDict={availableStory} />
//                 )}
//             </div>
//         </ModalWindow >
//     </>
//     )
// }


// function SearchDisplayInModal(props) {

//     return (
//         <button onClick={selectStory} className="displayStoryContainer">
//             <textarea value={firstSegment.earlier_segment_content} readOnly />
//             {(finalSegment != null) ? <textarea value={finalSegment.earlier_segment_content} readOnly /> : null}
//         </button>
//     )
// }



function AuthorListDisplayButton(props) {

    friendsDict = props.friendsDict;

    const [threeMostRecentAuthors, setThreeMostRecentAuthors] = useState([])

    const [authorArray, setAuthorArray] = useState([])

    const addAuthorTab = (authorID) => { props.etc }

    return (
        <div className="friendSearchContainer">
            {authorArray.map(authorDict =>
                <FriendProfileButton onClick={addAuthorTab} authorInfo={authorDict} />)}
        </div>

    )

}

function FriendProfileButton(props) {



    return (<button onClick={props.onClick}></button>)

}


```
So, regardless of backend, we still need frontend to have a "friends" dict.
As not coming from the top, can put in the dashboard object *IF* we like

[{authorID::, 
displayName::,

}]



```