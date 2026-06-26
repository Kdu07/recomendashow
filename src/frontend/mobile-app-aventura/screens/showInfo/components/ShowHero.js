import React from 'react';
import { View, StyleSheet } from 'react-native';
import FallbackImage from '../../../components/FallbackImage';

export default function ShowHero({ heroImage }) {
  return (
    <View style={styles.container}>
      <FallbackImage source={heroImage} style={styles.image} resizeMode="cover" />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    height: 260,
  },

  image: {
    width: '100%',
    height: '100%',
  },
});