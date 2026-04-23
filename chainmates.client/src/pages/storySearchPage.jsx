import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';

import { PageOrTabLayout } from "../layouts/layouts";
import { contactAPI } from "../supportFuncs/utilityFuncs";

export default { StorySearchPage, StorySubSearchPage };
export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyDict, setStoryDict] = useState(null);
    const [currentSegmentHistoryDTO, setCurrentSegmentHistoryDTO] = useState({});

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
        <PageOrTabLayout 
            topLine={
                <StorySeachPageTopLine storyDict={storyDict} />
            }
            mainContent ={ 
                <>
                    <div>{storyDict.title}</div>
                    <div>{storyDict.author.displayName}</div>
                    <Outlet />
                </>
            }
            footer={
                <>
                    <GoUpASegmentButton />
                    <GoDownASegmentButton />

                </>
            }
            rightSidebar={
                //<Comments selections={selectedSegmentDict} storyDict={storyDict} />
                null
            }
        /> 
    )
}

function StorySeachPageTopLine() { }


export function StorySubSearchPage() 
{ }



function GoUpASegmentButton() {
    const navigate = useNavigate();

    const onClick = () => {

    }
}