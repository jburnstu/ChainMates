
import React, { useState } from "react";
import { Link } from 'react-router-dom';

import { contactAPI } from "../supportFuncs/utilityFuncs";

export default { AuthorSearchButton, AuthorNameLink, StorySearchButton, StoryNameLink, FollowButton, UnFollowButton };

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
    console.log(authorInfo)
    return (
        <Link to={`/authors/${authorInfo.id}`}><button type="button">{authorInfo.displayName}</button></Link>
    )
}

export function StoryNameLink({ storyInfo }) {
    return (
        <Link to={`${storyInfo.id}`}><button type="button">{ storyInfo.title }</button ></Link>
    )
}

export function FollowButton(props) {

    async function handleSubmit(e) {
        console.log(props.authorDict)
        await contactAPI(`authors/whoyoufollow/${props.authorDict.id}`, "post", true)
    }

    return (
        <button onClick={handleSubmit}>FOLLOW
        </button>
    )
}

export function UnFollowButton(props) {

    async function handleSubmit(e) {
        await contactAPI(`authors/whoyoufollow/${props.authorDict.id}`, "delete", true,)
    }

    return (
        <button onClick={handleSubmit}>UNFOLLOW
        </button>
    )
}

