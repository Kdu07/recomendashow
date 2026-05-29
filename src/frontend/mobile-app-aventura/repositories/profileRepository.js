import {userProfile} from '../data/mockProfileData';
import {profileMenuItems} from '../constants/profileMenuItems';

export function getProfileData() {
  return {
    userProfile,
    profileMenuItems,
  };
}