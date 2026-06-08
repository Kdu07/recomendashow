import { userProfile as mockUserProfile } from '../data/mockProfileData';
import { profileMenuItems } from '../constants/profileMenuItems';
import { getUserInfo } from '../services/userProfileService';

export async function getProfileData() {
  try {
    const userProfile = await getUserInfo();
    return {
      userProfile,
      profileMenuItems,
    };
  } catch (error) {
    // fallback to mock data on error
    console.warn('Falling back to mock profile data:', error);
    return {
      userProfile: mockUserProfile,
      profileMenuItems,
    };
  }
}