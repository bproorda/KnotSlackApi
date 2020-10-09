import React, { useContext } from 'react';
import Button from 'react-bootstrap/Button';
import HubContext from '../../contexts/hubContext';
import CreateGroup from '../createGroup';
import './channelList.scss'


export default function ChannelList(props) {

    const { windows, doesWindowAlreadyExist } = useContext(HubContext);

    const clickHandler = (name) => {
        doesWindowAlreadyExist(name, "Group");
    }
//need to create different sections for channel type or use windows in userlist
    return (
        <div>
            <h3>Your Channels</h3>
            <CreateGroup />
            <ul style={{ listStyleType: "none", overflow: "auto", maxHeight: "50%" }}>
                {(windows !== null) ? windows.filter(channel => channel.type === "Group" || channel.type === "General").map((channel, index) => (
                    <li key={index}><Button onClick={() => clickHandler(channel.name)} >{channel.name}</Button></li>
                )) : null}
            </ul>
        </div>
    )
}