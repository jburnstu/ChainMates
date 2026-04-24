
import React, { useState } from "react";
import { createPortal } from 'react-dom';
import { useNavigate } from "react-router-dom";

import { contactAPI, getRandomItem } from "../supportFuncs/utilityFuncs";

export default { SubmissionButton, StartNewStoryButton, ModalSelectSegmentFromOptionsButton, ModalWindow };



async function getSegmentHistory(segmentID) {
    // Used universally to access this endpoint
    return await contactAPI(`segments/${segmentID}/history/`, "get", true);
}

async function createNewStoryWithInitialSegment(storyParameters) {
    // The API endpoint returns the initial segment that was created,
    // not the story object itself

    let initialSegmentData = await contactAPI("stories/", "post", true,
        storyParameters
    );
    return await getSegmentHistory(initialSegmentData.id);

}

async function uploadNewSegmentToStory(previousSegmentID) {
    // Currently just uses the previoussegmentid to fingure out other details like storyid

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

        createNewStoryWithInitialSegment(storyParameters)
            .then(function (value) {
                props.addNewStory(value)
                    .then(() => {
                        navigate(`/write/${value.id}/`) // Go to the story
                    })
            });
    }


    // Note that these story parameters aren't actually used to encode anything yet
    // -- it's an upcoming task. The handling of defaults / unused options also 
    // awaiting some TLC.
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


    async function handleSubmit(e) {
        let segmentHistoryData = await getSegmentHistory(props.segmentID);

        if (props.submissionType != "SAVE") {
            props.removeCurrentStory(segmentHistoryData);
        }

        // This is here to sidestep (if not really solve) issues of blank and or 
        // undefined content
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
                await contactAPI(`segments/moderationassignments/${props.segmentID}/approve`, "post", true
                );
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
    // Combined the buttons for joining a segment and moderating a new segment, as
    // the logic was exceptionally similar. This is starting to feel like not the best
    // choice, hence a lot of swtich-case-ing. Really certain methods need to be taken
    // out, and the buttons treated independently.

    let navigate = useNavigate();
    const numberOfChoices = 3; //WARNING this may be set elsewhere too


    ///////    Code to load up an array of options to join / moderate   ////////////////
    ////////// respectively (they're loaded up in a separate component)    /////////////
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
        // From the array of options (per this author), select n at random
        let availabilityData = await contactAPI(`segments/${apiArrayToAccess}/`, "get")
        let randomSegmentIDArray = await getRandomItem(availabilityData, numberOfChoices, true);


        // Asynchronously fill an array with the chosen segments' histories
        let segmentHistoryDTOArray = [];
        let segmentHistoryDTO;
        await Promise.all(randomSegmentIDArray.map(async (segmentID) => {
            segmentHistoryDTO = await getSegmentHistory(segmentID);
            segmentHistoryDTOArray.push(segmentHistoryDTO);
        }
        )
        )
        await setArrayOfAvailableStories(segmentHistoryDTOArray);
        return segmentHistoryDTOArray;
    }

    //Handle the user's choice of segment to join / moderate 
    function selectStory(previousSegmentID) {
        setIsOpen(false);

        console.log(previousSegmentID);
        switch (props.type) {
            case "JOIN":
                uploadNewSegmentToStory(previousSegmentID)
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


    ////// Code to set up the modal itself /////////////

    const [isOpen, setIsOpen] = useState(false);

    function createModal() {
        getSegmentsForModal()
            .then(function (value) {
                setIsOpen(true);
            })
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



export function ModalWindow(props) {

    if (!(props.isOpen)) return null;

    return (
        createPortal(
            // This is taken from an online example. It didn't work when I moved it to the CSS
            // file, so here it stays for now!
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