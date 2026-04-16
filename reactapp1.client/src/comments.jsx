
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
    let segmentTraceWithInfo = storyDict.segment_trace
    // segmentTraceWithInfo.map(segmentObj =>
    //     console.log(segmentObj.earlier_segment_id))

    return (
        <div className="comments">
            <StoryCommentPanel />
            {segmentTraceWithInfo.map(segmentObj =>
                <SegmentInfoPanel key={segmentObj.earlier_segment_id} selections={selections} segmentInfo={segmentObj} />
            )}
        </div>
    )
}

function StoryCommentPanel() { }


function SegmentInfoPanel(props) {

    let segmentInfo = props.segmentInfo;
    // console.log(segmentInfo);
    // console.log(props.selections)
    // console.log(props.selections[segmentInfo.earlier_segment_id]);

    const [isModerationOpen, setIsModerationOpen] = useState(false);

    function createComment() { }
    function submitComment() { }
    function abandonComment() { }

    return (<div className={`segmentInfoContainer ${props.selections[segmentInfo.earlier_segment_id] ? undefined : 'hidden'}`} >
        <div>{segmentInfo.earlier_segment_author.display_name}</div>
        <div className="moderationContainer">
            <button onClick={() => setIsModerationOpen(!isModerationOpen)}>LOOK AT MODERATION</button>
            <div className={isModerationOpen ? undefined : 'hidden'}>MODERATION PANEL</div>
        </div>
        <div className="segmentCommentsContainer">
            {segmentInfo.comments.map(segmentCommentObj =>
                <SegmentComment key={segmentCommentObj.id} segmentCommentInfo={segmentCommentObj} />)}
        </div>
        <div className="addCommentContainer">
            <button onClick={createComment}>+</button>
            <textarea></textarea>
            <button onClick={submitComment}>!</button>
            <button onClick={abandonComment}>X</button>
        </div>
    </ div >)
}


function SegmentComment(props) {

    let segmentCommentInfo = props.segmentCommentInfo;
    // console.log(segmentCommentInfo)

    return (
        <div className="segmentCommentContainer">{segmentCommentInfo.author.display_name}
            <textarea readOnly value={segmentCommentInfo.text_content} />
            <div className="commentCommentsContainer">
                {segmentCommentInfo.comments.map(commentCommentObj =>
                    <CommentComment key={commentCommentObj.id} commentCommentInfo={commentCommentObj} />
                )}
            </div>
            <div className="addCommentCommentContainer"></div>
        </div>)
}

function CommentComment(props) {

    return (
        <div className="commentCommentContainer">{props.commentCommentInfo.author.display_name}
            <textarea readOnly value={props.commentCommentInfo.text_content} />
        </div>
    )
}