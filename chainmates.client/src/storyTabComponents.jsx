
import React, { StrictMode, useState, useEffect } from "react";
import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet, useNavigate } from 'react-router-dom';
import { SubmissionButton } from './storyButtons.jsx';

import { Comments } from "./comments.jsx";
import { getArrayObjByID } from "./utilityFuncs";
import { contactAPI } from "./utilityFuncs";

export default { StoryTab , SegmentDisplay};

export function StoryTab(props) {

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

    //storyDict.segmentHistoryList.foreach((segmentDict) => {
    //    console.log(segmentDict);
    //                    });

    return (
        <div className="storyTabContainer tabContainer" id={"storyContainer" + { tabID }}>
            <StoryHeader storyDict={storyDict} wordCount={wordCount} />
            <div className="storyTabContent tabContent">
                {storyDict.segmentHistoryList.map(segmentDict =>
                    <SegmentDisplay key={segmentDict.id}
                        id={segmentDict.id}
                        isFinalSegment={segmentDict.id == tabID}
                        fixedContent={segmentDict.content}
                        currentContent={currentContent}
                        changeSelection={changeSegmentSelection}
                        onChange={handleChange} />
                )
                }
            </div>
            <SubmissionButtons writeOrReview={writeOrReview} currentContent={currentContent} segmentID={tabID} removeCurrentStory={removeCurrentStory} />
            <Comments selections={selectedSegmentDict} storyDict={storyDict} />
        </div>
    )
}

function StoryHeader(props) {

    let storyData = props.storyDict["storyData"];
    let length = props.storyDict.segmentHistoryList.length;

    return (<div className="storyTabHeader tabHeader">
        <div>{storyData.title ? storyData.title : "Untitled"}</div>
        <div>{"Section : " + length + (storyData.maxNumberOfSegments ? " / " + storyData.maxSnumberOfSegments : "")}</div>
        <div>{"Word Count :" + props.wordCount + (storyData.maxSegmentLength ? " / " + storyData.maxSegmentLength : "")}</div>
    </div>)
}

export function SegmentDisplay(props) {

    console.log(props.id)

    let readOnly = true;
    let onChange = null;
    let value = props.fixedContent;

    if (props.isFinalSegment) {
        readOnly = false;
        onChange = props.onChange;
        value = props.currentContent;
    }

    const onClick = () => {
        props.changeSelection(props.id)
    }


    return (
        <textarea className={`segmentDisplay ${readOnly ? undefined : 'currentSegmentDisplay'}`} readOnly={readOnly} value={value}
            onChange={onChange} onClick={onClick} ></ textarea>)

}


function SubmissionButtons(props) {

    let arrayOfButtonTypes;
    switch (props.writeOrReview) {
        case "review":
            arrayOfButtonTypes = ["APPROVE"];
            break;
        case "write":
        default:
            arrayOfButtonTypes = ["SAVE", "SUBMIT", "ABANDON"];
    }

    return (
        <div className="footer submissions">
            {arrayOfButtonTypes.map(buttonType =>
                <SubmissionButton
                    key={buttonType}
                    submissionType={buttonType}
                    currentContent={props.currentContent}
                    segmentID={props.segmentID}
                    removeCurrentStory={props.removeCurrentStory} />)}
        </div>
    )
}