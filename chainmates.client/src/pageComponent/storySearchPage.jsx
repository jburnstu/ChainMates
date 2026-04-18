import { getRandomItem, contactAPI, getArrayObjByID } from "./utilityFuncs";
import { AuthorContext } from "./context.js";
import React, { StrictMode, useState, authoref, useEffect, createContext, useContext } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';
import { SegmentDisplay } from "./workshopComponents";
import { FollowButton, UnFollowButton } from "./authorButtons";

export default { StorySearchPage };


export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyDict, setStoryDict] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            await contactAPI(`stories/${storyID}`, "get", false)
                .then(function (value) {
                    setStoryDict(value);
                })
        }

        if (storyID) {
            fetchData();
        }
    }, [storyID]);

    if (!storyDict) {
        if (storyDict != null) {
            console.log("DIDN@T WORK");
        }
        return null;
    }

    return (
        <>
            <div>{storyDict.title}</div>
            <div>{storyDict.author.displayName}</div>
        </>
    )

        <SubmissionButtons writeOrReview = { writeOrReview } currentContent = { currentContent } segmentID = { tabID } removeCurrentStory = { removeCurrentStory } />
        <Comments selections={selectedSegmentDict} storyDict={storyDict} />
}

