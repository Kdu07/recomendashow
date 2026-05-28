import React from 'react';
import { View, Text, StyleSheet, ScrollView, SafeAreaView } from 'react-native';

import SearchBar from '../components/SearchBar';
import CategoryChip from '../components/CategoryChip';
import SectionHeader from '../components/SectionHeader';
import EmptyState from '../components/EmptyState';
import SearchResultItem from '../components/SearchResultItem';

import useSearchViewModel from '../viewmodels/useSearchViewModel';

import { colors } from '../constants/colors';
import { typography } from '../constants/typography';
import { spacing, radius } from '../constants/spacing';

function getCategoryLabel(categoryId) {
  const labels = {
    musical: 'Musical',
    drama: 'Drama',
    infantil: 'Infantil',
    ballet: 'Ballet',
    hiphop: 'Hip Hop',
  };

  return labels[categoryId] || 'Espetáculo';
}

export default function SearchScreen() {
  const {
    searchText,
    selectedCategory,
    categories = [],
    filteredEvents = [],
    hasResults,
    sectionTitle,
    sectionSubtitle,
    handleSearchChange,
    handleCategoryPress,
    handleClearSearch,
    handleEventPress,
    handleTabPress,
  } = useSearchViewModel();

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.page}>
        <View style={styles.backgroundGlow} />

        <ScrollView
          showsVerticalScrollIndicator={false}
          contentContainerStyle={styles.scrollContent}
        >
          <View style={styles.header}>
            <Text style={styles.title}>Buscar</Text>
            <Text style={styles.subtitle}>
              Encontre espetáculos, teatros e experiências culturais.
            </Text>
          </View>

          <View style={styles.searchCard}>
            <SearchBar
              placeholder="Buscar por espetáculo, teatro ou cidade"
              value={searchText}
              onChangeText={handleSearchChange}
            />
          </View>

          <View style={styles.filterContainer}>
            <ScrollView
              horizontal
              showsHorizontalScrollIndicator={false}
              contentContainerStyle={styles.categoryRow}
            >
              {categories.map((category) => (
                <CategoryChip
                  key={category.id}
                  label={category.label}
                  selected={selectedCategory === category.id}
                  onPress={() => handleCategoryPress(category.id)}
                />
              ))}
            </ScrollView>
          </View>

          <View style={styles.resultsSection}>
            <SectionHeader
              title={sectionTitle}
              subtitle={sectionSubtitle}
            />

            {hasResults ? (
              <View style={styles.resultsList}>
                {filteredEvents.map((event) => (
                  <SearchResultItem
                    key={event.id}
                    title={event.title}
                    location={`${event.location} • ${event.city || 'Brasil'}`}
                    category={getCategoryLabel(event.categoryId)}
                    image={event.image}
                    onPress={() => handleEventPress(event)}
                  />
                ))}
              </View>
            ) : (
              <View style={styles.emptyCard}>
                <EmptyState
                  title="Nada encontrado"
                  description="Tente buscar por outro nome, teatro, cidade ou gênero."
                  actionLabel="Limpar busca"
                  onActionPress={handleClearSearch}
                />
              </View>
            )}
          </View>
        </ScrollView>

      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
  },

  page: {
    flex: 1,
    backgroundColor: colors.whiteSoft,
    position: 'relative',
  },

  backgroundGlow: {
    position: 'absolute',
    top: -110,
    right: -90,
    width: 240,
    height: 240,
    borderRadius: 999,
    backgroundColor: colors.purpleSoft,
    opacity: 0.3,
  },

  scrollContent: {
    paddingBottom: 155,
  },

  header: {
    paddingHorizontal: spacing.md,
    paddingTop: spacing.xl,
    gap: spacing.xs,
  },

  title: {
    fontSize: typography.size.xxl || 28,
    fontWeight: typography.fontWeight.bold,
    color: colors.grayDark,
  },

  subtitle: {
    fontSize: typography.size.sm,
    color: colors.grayText,
    lineHeight: typography.lineHeight.sm,
  },

  searchCard: {
    marginTop: spacing.xl,
    marginHorizontal: spacing.md,
    padding: spacing.sm,
    borderRadius: radius.lg,
    backgroundColor: 'rgba(255, 255, 255, 0.94)',
    borderWidth: 1,
    borderColor: colors.grayLight,

    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.05,
    shadowRadius: 12,
    elevation: 3,
  },

  filterContainer: {
    paddingTop: spacing.lg,
    paddingBottom: spacing.sm,
  },

  categoryRow: {
    paddingHorizontal: spacing.md,
    paddingRight: spacing.xl,
    gap: spacing.sm,
    alignItems: 'center',
  },

  resultsSection: {
    paddingHorizontal: spacing.md,
    marginTop: spacing.lg,
    gap: spacing.md,
  },

  resultsList: {
    width: '100%',
    borderRadius: radius.lg,
    backgroundColor: colors.white,
    paddingHorizontal: spacing.md,
    borderWidth: 1,
    borderColor: colors.grayLight,
    overflow: 'hidden',
  },

  emptyCard: {
    borderRadius: radius.lg,
    backgroundColor: colors.white,
    borderWidth: 1,
    borderColor: colors.grayLight,
    overflow: 'hidden',
  },
});