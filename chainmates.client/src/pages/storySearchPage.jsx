import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';

import { PageOrTabLayout } from "../layouts/layouts";
import { contactAPI } from "../supportFuncs/utilityFuncs";

export default { StorySearchPage, StorySubSearchPage };
export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyDict, setStoryDict] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            contactAPI(`stories/${storyID}`, "get", false)
                .then(function (value) {
                    setStoryDict(value);
                    console.log(value);
                    let firstSegmentID = value.strucutre[0];
                    navigate(`/stories/${storyID}/${firstSegmentID}/`);
                });
        }
        if (storyID) {
            fetchData();
        }
    }, [storyID]);


    ///////////////// Assign each segment an on-off selection for the Comments + Info sidebar ///////////////
    let noSelections = {};
    storyDict.segmentHistoryList.forEach(dictInArray =>
        noSelections[dictInArray.id] = false);
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);
    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }



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
                <Comments selections={selectedSegmentDict} storyDict={storyDict} />
                null
            }
        /> 
    )
}

function StorySeachPageTopLine() { }


export function StorySubSearchPage() 
{ 
    const { finalSegmentID } = useParams();

    const getSegmentHistory = async (finalSegmentId) => {
        return await contactAPI(`segments/${finalSegmentId}`, "get");
    }

    useEffect(() => {
        const getSegmentHistory = async (finalSegmentID) => {
            await contactAPI(`segments/${finalSegmentID}`, "get");
        }
        if (finalSegmentID) {
            getSegmentHistory(finalSegmentID);
        }
    }, [finalSegmentID]);




        return (
            <SegmentSeriesDisplay storyDict={storyDict}
                editableID={null}
                currentContent={null}
                changeSelection={changeSegmentSelection}
                onChange={null}
            /> 
    )
}
function GoUpASegmentButton() {
    const navigate = useNavigate();

    const onClick = () => {

    }
}