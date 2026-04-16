
import React, { StrictMode, useState, authoref, useEffect, createContext, useContext } from "react";
import { createRoot } from 'react-dom/client';
// import { BrowserRouter, Routes, Route, Link, Outlet, NavLink, useParams, useOutletContext, useOutlet } from 'react-router-dom';
// import { SubmissionButton, ModalJoinButton, ModalNewButton, NewModerationModalButton } from './storyButtons.jsx';
import { AuthorContext } from "./context.jsx";
import { getRandomItem, contactAPI } from "./utilityFuncs.jsx";
export default { Comments };


export function Comments(props) {

    let selections = props.selections;
    // console.log(selections)

    let storyDict = props.storyDict;
    let segmentTraceWithInfo = storyDict.segmentHistoryList
    // segmentTraceWithInfo.map(segmentObj =>
    //     console.log(segmentObj.id))

    return (
        <div className="rightSidebar comments">
            <StoryCommentPanel />
            {segmentTraceWithInfo.map(segmentObj =>
                <SegmentInfoPanel key={segmentObj.id} selections={selections} segmentInfo={segmentObj} />
            )}
        </div>
    )
}

function StoryCommentPanel(props) {
}


function SegmentInfoPanel(props) {

    let segmentInfo = props.segmentInfo;
     console.log(segmentInfo);
    // console.log(props.selections)
    // console.log(props.selections[segmentInfo.id]);

    const [isModerationOpen, setIsModerationOpen] = useState(false);



    return (<div className={`segmentInfoContainer ${props.selections[segmentInfo.id] ? undefined : 'hidden'}`} >
        <div>{segmentInfo.displayName}</div>
        <div className="moderationContainer">
            <button onClick={() => setIsModerationOpen(!isModerationOpen)}>LOOK AT MODERATION</button>
            <div className={isModerationOpen ? undefined : 'hidden'}>MODERATION PANEL</div>
        </div>
        <div className="segmentCommentsContainer">
            {segmentInfo.childComments.map(segmentCommentObj =>
                <SegmentComment key={segmentCommentObj.id} segmentCommentInfo={segmentCommentObj} />)}
        </div>
        <CommentCreationPanel parentType="segment" parentID={segmentInfo.id}/>
    </ div >)
}

function SegmentComment(props) {

    let segmentCommentInfo = props.segmentCommentInfo;
    console.log(segmentCommentInfo);

    return (
        <div className="segmentCommentContainer">{segmentCommentInfo.displayName}
            <textarea readOnly value={segmentCommentInfo.content} />
            <div className="commentCommentsContainer">
                {segmentCommentInfo.childComments.map(commentCommentObj =>
                    <CommentComment key={commentCommentObj.id} commentCommentInfo={commentCommentObj} />
                )}
            </div>
            <CommentCreationPanel parentType="comment" parentID={segmentCommentInfo.id} />
        </div>)
}

function CommentComment(props) {

    console.log(props.commentCommentInfo);

    return (
        <div className="commentCommentContainer">{props.commentCommentInfo.displayName}
            <textarea readOnly value={props.commentCommentInfo.content} />
        </div>
    )
}

function CommentCreationPanel(props) {
    const [currentContent, setCurrentContent] = useState("");
    const [isOpen, setIsOpen] = useState(false);

    let typeID;
    switch (props.parentType) {
        case "story":
            typeID = 1;
            break;
        case "segment":
            typeID = 2;
            break;
        case "comment":
            typeID = 3;
            break;
    }

    console.log(props.parentID, typeID);

    function onChange(e) {
        setCurrentContent(e.target.value);
    }

    async function createAndSubmitComment() {
        let commentSubmissionData = await contactAPI("comments/", "post", true,
            {
                commentTypeId: typeID,
                parentId: props.parentID,
                content: currentContent 
            });
    }


    return (
        <div className="addCommentContainer">
            <button onClick={() => setIsOpen(true)}>+</button>
            <div className= {isOpen? "" : "hidden" }>
                <textarea value={currentContent} onChange={onChange}></textarea>
                <button onClick={createAndSubmitComment}>!</button>
                <button onClick={() => setIsOpen(false)}>X</button>
            </div>
        </div>
    )

}
              