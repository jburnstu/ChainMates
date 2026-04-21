
import React, { useState } from "react";
import { createPortal } from 'react-dom';
import { useNavigate } from "react-router-dom";

import { contactAPI, getRandomItem } from "../supportFuncs/utilityFuncs";

export default { SubmissionButton, StartNewStoryButton, ModalSelectSegmentFromOptionsButton };



async function getSegmentHistory(segmentID) {
    // Used universally to access this endpoint
    return await contactAPI(`segments/${segmentID}/history/`, "get", true);
}

async function createNewStory(storyParameters) {
    // Creates both a story and a segment object; returns the new segment's historyDTO
    let initialSegmentData = await contactAPI("stories/", "post", true,
        storyParameters
    );
    return await getSegmentHistory(initialSegmentData.id);

}

async function uploadNewSegment(previousSegmentID) {

    let createSegmentData = await contactAPI("segments/","post",true,
        {
            'previousSegmentId': previousSegmentID
        }
    );
    return await getSegmentHistory(createSegmentData.id); 
}

async function uploadNewModerationAssignment(id) {
    //returns the historyDTO of the moderated segment

    await contactAPI(`segments/moderationassignments/${id}`,"post",true);
    return await getSegmentHistory(id);
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
    function createNewStory() {
        props.close();

        createNewStory(storyParameters)
            .then(function (value) {
                props.addNewStory(value)
                    .then(() => {
                        navigate(`/write/${value.id}/`)
                    })
            });
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
            <button type="button" onClick={createNewStory}>CREATE NEW STORY</button>
        </form >
    )
}

export function SubmissionButton(props) {
    let navigate = useNavigate();
    console.log(props);
    console.log(props.segmentID);

    async function handleSubmit(e) {
        let segmentHistoryData = await getSegmentHistory(props.segmentID);

        if (props.submissionType != "SAVE") {
            props.removeCurrentStory(segmentHistoryData);
        }
        console.log(props.currentContent);
        let currentContent = !props.currentContent ? "(Blank)" : props.currentContent;


        switch (props.submissionType) {
            case "SAVE":
                await contactAPI(`segments/${props.segmentID}/`, "patch", true,
                    {
                        'content': currentContent
                    }
                )
                break;
            case "SUBMIT":
                console.log("Submit", props.segmentID)
                await contactAPI(`segments/${props.segmentID}/submit/`, "post", true,
                    {
                        'content': currentContent
                    }
                )
                navigate(`/write/`);
                break;
            case "APPROVE":
                console.log("In approval");
                await contactAPI(`segments/moderationassignments/${props.segmentID}/approve`, "post", true);
                navigate(`/review/`);
                break;
            case "ABANDON":
                await contactAPI(`segments/${props.segmentID}/abandon/`, "post", true,
                    {
                        'content': currentContent
                    }
                )
                navigate(`/write/`);
                break;
        }
     
    }

    return (
        <button onClick={handleSubmit}>{props.submissionType}</button>
    )
}

export function ModalSelectSegmentFromOptionsButton(props) {
    let navigate = useNavigate();
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
        let availableIDArray = await contactAPI(`segments/${apiArrayToAccess}/`, "get")
        console.log(availableIDArray);
        let randomSegmentIDArray = await getRandomItem(availableIDArray, numberOfChoices, true);
        let segmentHistoryDTOArray = [];
        let segmentHistoryDTO;
        console.log(randomSegmentIDArray);

        await Promise.all(randomSegmentIDArray.map(async (segmentID) => {
            segmentHistoryDTO = await getSegmentHistory(segmentID);
            segmentHistoryDTOArray.push(segmentHistoryDTO);
        }
        )
        )
        await setArrayOfAvailableStories(segmentHistoryDTOArray);
        return segmentHistoryDTOArray;
    }

    function createModal() {
        getSegmentsForModal()
            .then(function (value) {
                setIsOpen(true);
            })
    }

    function selectStory(previousSegmentID) {
        setIsOpen(false);

        console.log(previousSegmentID);
        switch (props.type) {
            case "JOIN":
                uploadNewSegment(previousSegmentID)
                    .then(function (value) {
                        props.addNewStory(value)
                            .then(() => {
                                navigate(`/write/${value.id}`)
                            });
                    });
                break;
            case "MODERATE":
                uploadNewModerationAssignment(previousSegmentID)
                    .then(function (value) {
                        props.addNewStory(value)
                            .then(() => {
                                navigate(`/review/${value.id}`)
                            });
                    });
                break;
        }
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
    firstSegment = (finalSegment == firstSegment) ? null : firstSegment

    const selectStory = () => props.selectStory(finalSegment.id);

    return (
        <button onClick={selectStory} className="displayStoryContainer">
            <textarea value={(firstSegment != null) ? <textarea value={firstSegment.content} readOnly /> : null} readOnly />
            <textarea value={finalSegment.content} readOnly />
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