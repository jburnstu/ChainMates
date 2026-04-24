import React, { useEffect, useState } from "react";
import { useParams } from 'react-router-dom';

import { PageOrTabLayout } from "../layouts/layouts";
import { contactAPI } from "../supportFuncs/utilityFuncs";
import { Comments } from "../updates/comments"
import { ModalWindow} from "../buttons/workshopButtons"

import { SegmentDisplayInModal, SegmentSeriesDisplay } from "../segmentDisplay"

export default { StorySearchPage};

export function StorySearchPage() {

    const { storyID } = useParams();
    const [storyInfo, setStoryInfo] = useState(null);
    const [finalSegmentDTO, setFinalSegmentDTO] = useState(null);

    async function changeFinalSegment(finalSegmentID) {
        await contactAPI(`segments/${finalSegmentID}/history`, "get")
            .then(function (value) {
                setFinalSegmentDTO(value);
            })
    }

    useEffect(() => {
        const fetchData = async (storyID) => {
            await contactAPI(`stories/${storyID}`, "get", true)
                .then(function (value) {
                    console.log(value);
                    setStoryInfo(value);
                    changeFinalSegment(value.structure[0][0]); // The first segment in the story is saved at 0 (in an array of one)
                    //navigate(`/stories/${storyID}/${firstSegmentID}/`);
                });
        }
        console.log("In useeffect")
        if (storyID) {
            fetchData(storyID);
        }
    }, [storyID]);



    ///////////////// Assign each segment an on-off selection for the Comments + Info sidebar ///////////////
    let noSelections = {};
    console.log(finalSegmentDTO)
    if (finalSegmentDTO) {
        finalSegmentDTO.segmentHistoryList.forEach(dictInArray =>
            noSelections[dictInArray.id] = false);
    }
    const [selectedSegmentDict, setSelectedSegmentDict] = useState(noSelections);
    function changeSegmentSelection(segmentID) {
        setSelectedSegmentDict({ ...selectedSegmentDict, [segmentID]: !selectedSegmentDict[segmentID] })
    }

    if (!storyInfo || !finalSegmentDTO) {
        return null;
    }
    console.log(finalSegmentDTO)

    return (
        <PageOrTabLayout 
            topLine={
                <StorySeachPageTopLine storyInfo={storyInfo} />
            }
            mainContent={
                <SegmentSeriesDisplay segmentHistoryList={finalSegmentDTO.segmentHistoryList}
                    editableID={null}
                    currentContent={null}
                    changeSelection={changeSegmentSelection}
                    onChange={null}
                /> 
            }
            footer={
                <>
                    <GoUpASegmentButton previousSegmentList={finalSegmentDTO.segmentHistoryList}
                        changeFinalSegment={changeFinalSegment} />
                    <GoDownASegmentButton possibleSegmentIDList={storyInfo.structure[finalSegmentDTO.id]}
                        changeFinalSegment={changeFinalSegment}/>
                </>
            }
            rightSidebar={
                <Comments selections={selectedSegmentDict} storyDict={finalSegmentDTO} />
            
            }
        /> 
    )
}

function StorySeachPageTopLine() { }

function GoDownASegmentButton(props) {
    console.log(props.possibleSegmentIDList)
    const [isOpen, setIsOpen] = useState(false);
    const [arrayOfAvailableSegments, setArrayOfAvailableSegments] = useState([]);

    function createModal() {
        setIsOpen(true);
        console.log("modal clicked")
    }

    useEffect(() => {
        async function getSegmentsForModal(possibleSegmentIDList) {
            console.log(possibleSegmentIDList);
            let possibleSegmentDTOList = [];
            let segmentHistoryDTO;
            await Promise.all(possibleSegmentIDList.map(async (segmentID) => {
                segmentHistoryDTO = await contactAPI(`segments/${segmentID}/history`, "get", true);
                possibleSegmentDTOList.push(segmentHistoryDTO);
                console.log(segmentHistoryDTO);
            }
            )
            )
            await setArrayOfAvailableSegments(possibleSegmentDTOList);
            return possibleSegmentDTOList;
        }
        if (props.possibleSegmentIDList != undefined) {
            getSegmentsForModal(props.possibleSegmentIDList)
        }
    }, [props.possibleSegmentIDList])

    return (
        <>
            <button onClick={createModal}> Continue Reading...
            </ button >
            <ModalWindow isOpen={isOpen} onClose={() => setIsOpen(false)}>
                <div className="allDisplayStoriesContainer">
                    {arrayOfAvailableSegments.map(availableSegment =>
                        <SegmentDisplayInModal
                            key={availableSegment.id}
                            selectStory={() => props.changeFinalSegment(availableSegment.id)}
                            storyDict={availableSegment}
                        />
                    )}
                </div>
            </ModalWindow >
        </>
    )

}

function GoUpASegmentButton(props) {
    console.log("HERE",props.previousSegmentList)

    let previousSegmentID = props.previousSegmentList.slice(-2)[0]?.id
    return (
        <button onClick={() => {
            if (!previousSegmentID) {
                console.log("No psi")
                return;
            }
            props.changeFinalSegment(previousSegmentID);
        }
        }
        />
    )
}




//export function StorySubSearchPage() {
//    const { finalSegmentID } = useParams();

//    return (
//        <SegmentSeriesDisplay storyDict={storyDict}
//            editableID={null}
//            currentContent={null}
//            changeSelection={changeSegmentSelection}
//            onChange={null}
//        />
//    )
//}