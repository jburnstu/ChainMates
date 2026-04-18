import React, { StrictMode, useState, useEffect } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams } from 'react-router-dom';

import { contactAPI } from "./utilityFuncs";
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
        <TabOrPageLayout 
            topLine={
                <StorySeachPageTopLine storyDict={storyDict} />
            }
            mainContent ={ 
                <>
                    <div>{storyDict.title}</div>
                    <div>{storyDict.author.displayName}</div>
                </>
            }
            footer={
                null
            }
            rightSidebar={
                //<Comments selections={selectedSegmentDict} storyDict={storyDict} />
                null
            }
        /> 
    )
}

function StorySeachPageTopLine() { }