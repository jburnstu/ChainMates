
import React, { useState } from "react";
import { createPortal } from 'react-dom';
import { useNavigate } from "react-router-dom";

import { getRandomItem, contactAPI } from "./utilityFuncs.jsx";

export default { SubmissionButton, StartNewStoryButton, ModalSelectSegmentFromOptionsButton };

async function uploadNewSegment(previousSegmentID) {

    let updatePreviousSegmentData = await contactAPI(`segments/${previousSegmentID}/`,"patch",true,
        {
            'segmentStatusId': 5
        }
    );
    let createSegmentData = await contactAPI("segments/","post",true,
        {
            'storyId': updatePreviousSegmentData.storyId,
            'segmentStatusId': 1,
            'previousSegmentId': previousSegmentID
        }
    );
    let getNewFullStoryInfoData = await contactAPI(`segments/traces/${createSegmentData.id}`, "get",true);
    return getNewFullStoryInfoData
}

async function uploadNewStoryAndSegment(storyParameters) {

    let storyCreationData = await contactAPI("stories/", "post",true,
        storyParameters
    );
    let createSegmentData = await contactAPI("segments/", "post", true,
        {
            'storyId': storyCreationData.id
        }
    )
    let getNewFullStoryInfoData = await contactAPI(`segments/traces/${createSegmentData.id}`, "get",true);
    return getNewFullStoryInfoData
}

async function uploadNewModerationAssignment(previousSegmentID) {
    await contactAPI("segments/", "patch",
        {
            "segmentStatusId": 3
        }
    );
    let moderationAssignmentCreationData = await contactAPI("moderationassignments/", "post",true,
        {
            'segmentId': previousSegmentID
        }
    )
    return moderationAssignmentCreationData;
}

export function StartNewStoryButton(props) {
    const [isOpen, setIsOpen] = useState(false);

    function createModal() {
        setIsOpen(true);
        console.log("modal clicked")
    }

    return (
        <>
            <button onClick={createModal}> Start A New Story
            </ button >
            <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
                <div className="allDisplayStoriesContainer">
                    <NewStoryOptionspanel addNewStory={props.addNewStory} close={() => setIsOpen(false)} />
                </div>
            </ModalWindow >
        </>
    )
}

function NewStoryOptionspanel(props) {
    let navigate = useNavigate();
    const urlStub = `/chainlettersstories/`;

    const [storyParameters, setStoryParameters] = useState({});
    const [parameterChecks, setParameterChecks] = useState({});

    const handleValueChange = (e) => {
        const name = e.target.name;
        const value = (e.target.type == "checkbox") ? e.target.checked : e.target.value;
        setStoryParameters(values => ({ ...values, [name]: value }))
    }
    const handleCheckChange = (e) => {
        const name = e.target.name;
        const checked = e.target.checked;
        setParameterChecks(values => ({ ...values, [name]: checked }))
    }

    function createNewStoryAndSegment() {
        props.close()
        uploadNewStoryAndSegment(storyParameters)
            .then(function (value) {
                props.addNewStory(value)
                    .then(function (innerValue) {
                        navigate(`/write/${value.id}/`)
                    })
            }
            )
    }

    return (
        <form>
            <fieldset className="newStoryModalFieldset">
                <input type="text" name="title"
                    value={storyParameters.storyTitle}
                    defaultValue="Title"
                    onChange={handleValueChange}></input >
                <label>Min. Segment Length
                    <input label="Min. Segment Length" type="checkbox" name="checkMinSegmentLength"
                        checked={parameterChecks.checkMinSegmentLength}
                        onChange={handleCheckChange}>
                    </input>
                    <input defaultValue="200" type="number" name="MinSegmentLength"
                        disabled={!parameterChecks.checkMinSegmentLength}
                        value={storyParameters.MinSegmentLength}
                        onChange={handleValueChange}>
                    </input>Words
                </label>
                <label>Max. Segment Length<input type="checkbox" name="checkMaxSegmentLength"
                    checked={storyParameters.checkMaxSegmentLength}
                    onChange={handleCheckChange}></input>
                    <input defaultValue="200" type="number" name="maxSegmentLength"
                        value={storyParameters.maxSegmentLength}
                        disabled={!parameterChecks.checkMaxSegmentLength}
                        onChange={handleValueChange}></input>
                    Words</label>
                <label >Max. Number of Segments?<input type="checkbox" name="checkMaxNumberOfSegments"
                    checked={storyParameters.checkMaxNumberOfSegments}
                    onChange={handleCheckChange}></input>
                    <input defaultValue="200" type="number" name="maxNumberOfSegments"
                        value={storyParameters.maxNumberOfSegments}
                        disabled={!parameterChecks.checkMaxNumberOfSegments}
                        onChange={handleValueChange}></input>
                    Segments</label>
                <label>Max. Number of Branches?<input type="checkbox" name="checkMaxNumberOfBranches"
                    checked={storyParameters.checkMaxNumberOfBranches}
                    onChange={handleCheckChange}></input>
                    <input defaultValue="2" type="number" name="maxNumberOfBranches"
                        disabled={!parameterChecks.checkMaxNumberOfBranches}
                        value={storyParameters.maxNumberOfBranches}
                        onChange={handleValueChange}></input>
                    Branches</label>
                <label>Mature<input type="checkbox"
                    name="isItMature"
                    checked={storyParameters.isItMature}
                    onChange={handleValueChange}></input>
                </label>
            </fieldset>
            <button type="button" onClick={createNewStoryAndSegment}>CREATE NEW STORY</button>
        </form >
    )
}

export function SubmissionButton(props) {
    let navigate = useNavigate();

    let segmentStatusID;
    switch (props.submissionType) {
        case "SAVE":
            segmentStatusID = 1;
            break;
        case "SUBMIT":
            segmentStatusID = 2;
            break;
        case "APPROVE":
            segmentStatusID = 4;
            break;
        case "ABANDON":
            segmentStatusID = 6;
            break;
    }

    async function handleSubmit(e) {
        let getNewFullStoryInfoData = await contactAPI(`segments/traces/${props.segmentID}`, "get");

        if (props.submissionType != "SAVE") {
            props.removeCurrentStory(getNewFullStoryInfoData);
        }
        let currentContent = !props.currentContent ? "(Blank)" : props.currentContent;

        contactAPI(`segments/${props.segmentID}/`, "patch",
            {
                'segmentStatusId': segmentStatusID,
                'content': currentContent
            }
        )
        .then(
            function (value) {
                if (props.submissionType == "ABANDON") {
                    if (value.previousSegmentId != null) {
                        contactAPI(`segments/${value.previousSegmentId}/`, "patch",
                            {
                                'segmentStatusId': 4
                            }
                        )
                    }
                }
            })
        navigate(`/write`);
    }

    return (
        <button onClick={handleSubmit}>{props.submissionType}</button>
    )
}

export function ModalSelectSegmentFromOptionsButton(props) {
    let navigate = useNavigate();
    const urlStub = `/chainlettersstories/`;
    const numberOfChoices = 3;

    const [isOpen, setIsOpen] = useState(false);
    const [arrayOfAvailableStories, setArrayOfAvailableStories] = useState([]);

    let apiArrayToAccess;
    switch (props.type) {
        case "MODERATE":
            apiArrayToAccess = "moderatablesegments";
            break;
        case "JOIN":
            apiArrayToAccess = "joinablesegments";
            break;
    }

    async function getSegmentsForModal() {
        console.log("in getSegmentsForModal")
        let availabilityData = await contactAPI(`segments/${apiArrayToAccess}/`, "get")
        let randomSegmentIDArray = await getRandomItem(availabilityData, numberOfChoices);
        let segmentTraceDataArray = [];
        let segmentTraceData;
        await Promise.all(randomSegmentIDArray.map(async (segmentID) => {
            segmentTraceData = await contactAPI(`segments/traces/${segmentID}`, "get");
            segmentTraceDataArray.push(segmentTraceData);
        }
        )
        )
        await setArrayOfAvailableStories(segmentTraceDataArray);
        return segmentTraceDataArray;
    }

    function createModal() {
        getSegmentsForModal()
            .then(function (value) {
                setIsOpen(true);
            })
    }

    function selectStory(previousSegmentID) {
        setIsOpen(false);
        uploadNewSegment(previousSegmentID)
            .then(function (value) {
                props.addNewStory(value)
                    .then(function (innerValue) {
                        navigate(`/write/${value.id}`)
                    });
            });
    }

    return (
        <>
            <button onClick={createModal}>{props.type}
            </ button >
            <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
                <div className="allDisplayStoriesContainer">
                    {arrayOfAvailableStories.map(availableStory =>
                        <SegmentDisplayInModal key={availableStory.id} selectStory={selectStory} storyDict={availableStory} />
                    )}
                </div>
            </ModalWindow >
        </>
    )
}

function SegmentDisplayInModal(props) {
    console.log(props.storyDict);
    let firstSegment = props.storyDict.segmentHistoryList[0]
    let finalSegment = props.storyDict.segmentHistoryList.slice(-1)[0]
    finalSegment = (finalSegment == firstSegment) ? null : finalSegment

    const selectStory = () => props.selectStory(finalSegment.id);

    return (
        <button onClick={selectStory} className="displayStoryContainer">
            <textarea value={firstSegment.content} readOnly />
            {(finalSegment != null) ? <textarea value={finalSegment.content} readOnly /> : null}
        </button>
    )
}



function ModalWindow(props) {

    if (!(props.isOpen)) return null;

    return (
        createPortal(
            <div style={{
                position: 'fixed',
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                backgroundColor: 'rgba(0, 0, 0, 0.5)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
            }}>
                {props.children}
                <div className="modalButtonContainer">
                    <button onClick={props.onClose}>Close</button>
                </div>
            </div>,
            document.body
        )
    )
}