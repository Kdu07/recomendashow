import React from 'react';
import { View, Image, StyleSheet } from 'react-native';

export default function ShowHero({ heroImage }) {
  return (
    <View style={styles.container}>
      <Image source={heroImage} style={styles.image} resizeMode="cover" />
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
