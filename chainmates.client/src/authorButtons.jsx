
import React, { useState, useEffect, useContext } from "react";
import { createPortal } from 'react-dom';
import { AuthorContext } from "./context.jsx";
import { useNavigate, useLocation, redirect } from "react-router";
import { getRandomItem, contactAPI } from "./utilityFuncs.jsx";

export default { AuthorSearchButton, AuthorNameLink, StorySearchButton, StoryNameLink };

export function AuthorSearchButton() {

    const [isOpen, setIsOpen] = useState(false);
    const [authorArray, setAuthorArray] = useState([])

    async function onClick() {
        if (!isOpen) {
            contactAPI(`authors`, "get", false)
                .then(function (value) {
                    console.log(value)
                    setAuthorArray(value)
                })
                .then(function (innerValue) {
                    setIsOpen(true);
                })
        }
        else { setIsOpen(false) }
    }

    return (
        <div className="authorSearchButton">
            <button onClick={onClick}>AUTHORS</button>
            {isOpen 
               ? <div className="searchContainer">
                    {authorArray.map(authorDict =>
                        <div className="searchResultContainer">
                            <AuthorNameLink key={authorDict.id} authorInfo={authorDict} />)
                        </div>
                    )}
                </div>
               : null}
        </div>
    )
}

export function AuthorNameLink({ authorInfo }) {
    return (
        <Link to={`authors/${authorInfo.Id}`}>{authorInfo.DisplayName}</Link>
    )
}




export function StorySearchButton() {

    const [isOpen, setIsOpen] = useState(false);
    const [storyArray, setStoryArray] = useState([])

    async function onClick() {
        if (!isOpen) {
            contactAPI(`stories`, "get", false)
                .then(function (value) {
                    console.log(value)
                    setStoryArray(value)
                })
                .then(function (innerValue) {
                    setIsOpen(true);
                })
        }
        else { setIsOpen(false) }
    }

    return (
        <div className="storySearchButton">
            <button onClick={onClick}>STORIES</button>
            {isOpen
                ? <div className="searchContainer">
                    {storyArray.map(storyDict =>
                        <div className="searchResultContainer">
                            <StoryNameLink key={storyDict.Id} storyInfo={storyDict} />)
                            <AuthorNameLink key={storyDict.Id} authorInfo={storyDict.Author} />
                        </div>
                    )}
                </div>
                : null}
        </div>
    )
}

export function StoryNameLink({ storyInfo }) {
    return (
        <Link to={`authors/${storyInfo.Id}`}>{storyInfo.Title}</Link>
    )
}



//export function AuthorListDisplayButton(props) {
//    const authorID = useContext(AuthorContext);

//    const [threeMostRecentAuthors, setThreeMostRecentAuthors] = useState([])

//    const [isOpen, setIsOpen] = useState(false);

//    const [authorArray, setAuthorArray] = useState([])

//    const onClick = () => {
//        if (!isOpen) {
//            contactAPI(`author_relation_by_author/${authorID}/`, "get")
//                .then(function (value) {
//                    console.log(value)
//                    setAuthorArray(value.related_authors)
//                })
//                .then(function (innerValue) {
//                    setIsOpen(true);
//                }
//                )
//        }
//        else { setIsOpen(false) }
//    }

//    return (
//        <div className="friendSearchContainer">
//            <button onClick={onClick}>AUTHORS</button>
//            {authorArray.map(authorDict =>
//                <FriendProfileButton key={authorDict.id} addAuthorTab={props.addAuthorTab} authorInfo={authorDict} />)}
//        </div>
//    )
//}

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



//function AuthorListDisplayButton(props) {

//    friendsDict = props.friendsDict;

//    const [threeMostRecentAuthors, setThreeMostRecentAuthors] = useState([])

//    const [authorArray, setAuthorArray] = useState([])

//    const addAuthorTab = (authorID) => { props.etc }

//    return (
//        <div className="friendSearchContainer">
//            {authorArray.map(authorDict =>
//                <FriendProfileButton onClick={addAuthorTab} authorInfo={authorDict} />)}
//        </div>

//    )

//}

