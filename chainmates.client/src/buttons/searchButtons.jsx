
import React, { useState } from "react";
import { Link } from 'react-router-dom';

import { contactAPI } from "../utilityFuncs";

export default { AuthorStringLink, AuthorSearchButton, AuthorNameLink, StorySearchButton, StoryNameLink, FollowButton, UnFollowButton };

export function AuthorSearchButton() {
    // Currently just loads up all authors (no filtering)

    const [isOpen, setIsOpen] = useState(false);
    const [authorArray, setAuthorArray] = useState([])

    async function onClick() {
        if (!isOpen) {
            contactAPI(`authors`, "get", false)
                .then(function (value) {
                    setAuthorArray(value)
                })
                .then(() => {
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
                        <div className="searchResultContainer" key={authorDict.id}>
                            <AuthorNameLink authorInfo={authorDict} />
                        </div>
                    )}
                </div>
               : null}
        </div>
    )
}

export function StorySearchButton() {
    // Same as above, except also loads up the author who began the story
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
                        <div className="searchResultContainer" key={storyDict.id}>
                            <StoryNameLink storyInfo={storyDict} />)
                            <AuthorNameLink authorInfo={storyDict.author} />
                        </div>
                    )}
                </div>
                : null}
        </div>
    )
}


export function AuthorNameLink({ authorInfo }) {
    // Would be nice to turn this into something more universal, i.e. everywhere you see their name
    return (
        <Link to={`/authors/${authorInfo.id}`}><button type="button">{authorInfo.displayName}</button></Link>
    )
}

export function AuthorStringLink({ authorInfo }) {
    console.log(authorInfo)
    return (
        <Link to={`/authors/${authorInfo.Id}`}>{authorInfo.DisplayName}</Link>
    )
}
export function StoryNameLink({ storyInfo }) {
    // Same as above
    return (
        <Link to={`${storyInfo.id}`}><button type="button">{ storyInfo.title }</button ></Link>
    )
}

export function FollowButton(props) {
    // Need something to prevent double-follow / self-follow 
    async function handleSubmit(e) {
        await contactAPI(`authors/whoyoufollow/${props.authorDict.id}`, "post", true)
    }

    return (
        <button onClick={handleSubmit}>FOLLOW
        </button>
    )
}

export function UnFollowButton(props) {
    // Need to prevent double-un-follow

    async function handleSubmit(e) {
        await contactAPI(`authors/whoyoufollow/${props.authorDict.id}`, "delete", true,)
    }

    return (
        <button onClick={handleSubmit}>UNFOLLOW
        </button>
    )
}

