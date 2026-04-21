
import React, { StrictMode, useState } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext } from 'react-router-dom';

import { getArrayObjByID } from "../supportFuncs/utilityFuncs";

import { SubmissionButton } from '../buttons/workshopButtons';

import { Comments } from "../updates/comments";

import { PageOrTabLayout } from "../layouts/layouts"

import { SegmentDisplay } from "../segmentDisplay"

export default { WorkshopTab };

export function WorkshopTab(props) {

    let writeOrReview = props.writeOrReview;
    const { tabID } = useParams();
    const [wordCount, setWordCount] = useState(0);
    const [currentContentByStory, setCurrentContentByStory] = useOutletContext();

    console.log(tabID, props.dicts);


    let storyDict = getArrayObjByID(props.dicts, tabID);
    let currentContent = currentContentByStory[tabID];

    let noSelections = {};
    storyDict.segmentHistoryList.forEach(dictInArray =>
        noSelections[dictInArray.id] = false)
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);

    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }

    function handleChange(e) {
        setCurrentContentByStory({ ...currentContentByStory, [tabID]: e.target.value });
        setWordCount(getWordCount(currentContent));
    }

    function getWordCount(myText) {
        // const spaceMatchPattern = /[\w\d][\s\W*\d*]+[\w\d]/;
        const spaceMatchPattern = /\S+/g;
        let numberOfSpaces = myText.match(spaceMatchPattern);
        return (numberOfSpaces ? numberOfSpaces : []).length;
    }

    const removeCurrentStory = (storyDict) => props.setDicts(storyDict, writeOrReview, "remove");

    return (
        <PageOrTabLayout 
            topLine={
                <WorkshopStoryHeader storyDict={storyDict} wordCount={wordCount} />
            }
            mainContent ={ 
                storyDict.segmentHistoryList.map(segmentDict =>
                    <SegmentDisplay key={segmentDict.id}
                        id={segmentDict.id}
                        isFinalSegment={segmentDict.id == tabID}
                        fixedContent={segmentDict.content}
                        currentContent={currentContent}
                        changeSelection={changeSegmentSelection}
                        onChange={handleChange}/>
                )
            }
            footer={
                props.type == "write"
                    ?
                        ["SAVE", "SUBMIT", "ABANDON"].map(buttonType =>
                            <SubmissionButton
                                key={buttonType}
                                submissionType={buttonType}
                                currentContent={props.currentContent}
                                segmentID={props.segmentID}
                                removeCurrentStory={removeCurrentStory} />)
                    :
                        ["APPROVE"].map(buttonType =>
                            <SubmissionButton
                                key={buttonType}
                                submissionType={buttonType}
                                currentContent={props.currentContent}
                                segmentID={props.segmentID}
                                removeCurrentStory={removeCurrentStory} />)
            }
            rightSidebar={
                <Comments selections={selectedSegmentDict} storyDict={storyDict} />
            }
        /> 
    )
}

function WorkshopStoryHeader(props) {

    let storyData = props.storyDict["storyData"];
    let length = props.storyDict.segmentHistoryList.length;

    return (
        <>
            <div>{storyData.title ? storyData.title : "Untitled"}</div>
            <div>{"Section : " + length + (storyData.maxNumberOfSegments ? " / " + storyData.maxSnumberOfSegments : "")}</div>
            <div>{"Word Count :" + props.wordCount + (storyData.maxSegmentLength ? " / " + storyData.maxSegmentLength : "")}</div>
        </>
    )
}

